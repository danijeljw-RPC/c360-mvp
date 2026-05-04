using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Data.Entities;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Services;

public sealed class BillingService(AppDbContext db, AuditService audit, InvoiceNumberService invoiceNumbers)
{
    public async Task<InternalInvoice> GenerateInvoiceAsync(Guid bookingId, string actorEmail)
    {
        var booking = await db.Bookings
            .Include(x => x.TravelPolicy).ThenInclude(p => p!.SavedPaymentMethod)
            .Include(x => x.ClientOrganisation)
            .Include(x => x.TmcOrganisation)
            .FirstOrDefaultAsync(x => x.Id == bookingId)
            ?? throw new InvalidOperationException("Booking not found");

        if (booking.Status == BookingStatus.NeedsReview)
            throw new InvalidOperationException("Booking needs review before invoicing");

        // Determine collection mode and provider from travel policy
        var (collectionMode, providerType) = ResolveCollectionMode(booking.TravelPolicy);

        var invoiceNumber = await invoiceNumbers.NextAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var invoice = new InternalInvoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = invoiceNumber,
            SellerOrganisationId = booking.TmcOrganisationId,
            BuyerOrganisationId = booking.ClientOrganisationId,
            BookingId = bookingId,
            Status = InvoiceStatus.Draft,
            CollectionMode = collectionMode,
            PaymentProviderType = providerType,
            IssueDate = today,
            DueDate = today.AddDays(14),
            CurrencyCode = booking.CurrencyCode,
            SubtotalAmount = booking.SupplierAmount + booking.ServiceFeeAmount,
            TaxAmount = booking.TaxAmount,
            TotalAmount = booking.TotalAmount,
        };

        invoice.Lines.Add(new InternalInvoiceLine
        {
            Id = Guid.NewGuid(),
            LineType = InvoiceLineType.SupplierFare,
            Description = $"Supplier fare — {booking.PnrCode}",
            Quantity = 1,
            UnitAmount = booking.SupplierAmount,
            TaxAmount = booking.TaxAmount,
            LineTotalAmount = booking.SupplierAmount + booking.TaxAmount
        });

        if (booking.ServiceFeeAmount > 0)
        {
            invoice.Lines.Add(new InternalInvoiceLine
            {
                Id = Guid.NewGuid(),
                LineType = InvoiceLineType.BookingFee,
                Description = "TMC booking/service fee",
                Quantity = 1,
                UnitAmount = booking.ServiceFeeAmount,
                TaxAmount = 0,
                LineTotalAmount = booking.ServiceFeeAmount
            });
        }

        if (booking.SurchargeAmount > 0)
        {
            invoice.Lines.Add(new InternalInvoiceLine
            {
                Id = Guid.NewGuid(),
                LineType = InvoiceLineType.CardSurcharge,
                Description = "Card surcharge",
                Quantity = 1,
                UnitAmount = booking.SurchargeAmount,
                TaxAmount = 0,
                LineTotalAmount = booking.SurchargeAmount
            });
        }

        invoice.Status = InvoiceStatus.Issued;
        db.InternalInvoices.Add(invoice);

        db.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            SellerOrganisationId = invoice.SellerOrganisationId,
            BuyerOrganisationId = invoice.BuyerOrganisationId,
            BookingId = bookingId,
            InternalInvoiceId = invoice.Id,
            EntryType = LedgerEntryType.InvoiceRaised,
            Description = $"Invoice {invoiceNumber} raised for booking {booking.BookingReference}",
            DebitAmount = invoice.TotalAmount,
            CreditAmount = 0,
            CurrencyCode = invoice.CurrencyCode
        });

        booking.Status = BookingStatus.Invoiced;
        await db.SaveChangesAsync();

        await audit.LogAsync(actorEmail, "Invoice", invoice.Id, "InvoiceGenerated",
            $"Invoice {invoiceNumber} generated for booking {booking.BookingReference}. Collection mode: {collectionMode}");

        return invoice;
    }

    private static (InvoiceCollectionMode mode, PaymentProviderType provider) ResolveCollectionMode(TravelPolicy? policy)
    {
        if (policy == null) return (InvoiceCollectionMode.AutomaticProviderCollection, PaymentProviderType.Stripe);
        return policy.BillingBehaviour switch
        {
            TravelPolicyBillingBehaviour.ManualInvoice => (InvoiceCollectionMode.ManualCollection, PaymentProviderType.ManualInvoice),
            TravelPolicyBillingBehaviour.ExternalSettlement => (InvoiceCollectionMode.ExternalFinanceSystem, PaymentProviderType.ExternalProcessor),
            TravelPolicyBillingBehaviour.ShadowBilling => (InvoiceCollectionMode.ShadowOnly, PaymentProviderType.ShadowBilling),
            TravelPolicyBillingBehaviour.NoCharge => (InvoiceCollectionMode.NoCharge, PaymentProviderType.NoCharge),
            _ => (InvoiceCollectionMode.AutomaticProviderCollection, PaymentProviderType.Stripe)
        };
    }
}
