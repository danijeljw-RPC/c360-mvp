using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Data.Entities;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Services;

public sealed class PaymentSimulationService(AppDbContext db, AuditService audit)
{
    private static int _piCounter = 1;
    private static readonly Lock _lock = new();

    public async Task<(bool succeeded, string message)> SimulateAsync(Guid invoiceId, string actorEmail)
    {
        var invoice = await db.InternalInvoices
            .Include(x => x.SellerOrganisation)
            .Include(x => x.BuyerOrganisation)
            .Include(x => x.Booking).ThenInclude(b => b!.TravelPolicy).ThenInclude(p => p!.SavedPaymentMethod)
            .Include(x => x.PaymentAttempts)
            .FirstOrDefaultAsync(x => x.Id == invoiceId);

        if (invoice == null) return (false, "Invoice not found");

        if (invoice.CollectionMode == InvoiceCollectionMode.ManualCollection)
        {
            invoice.Status = InvoiceStatus.PaymentPending;
            await db.SaveChangesAsync();
            await audit.LogAsync(actorEmail, "Invoice", invoiceId, "ManualInvoiceIssued", $"Invoice {invoice.InvoiceNumber} set to awaiting manual payment");
            return (true, "Manual invoice — awaiting manual settlement");
        }

        if (invoice.CollectionMode == InvoiceCollectionMode.ExternalFinanceSystem)
        {
            invoice.Status = InvoiceStatus.ExternallyManaged;
            await db.SaveChangesAsync();
            await audit.LogAsync(actorEmail, "Invoice", invoiceId, "ExternalSettlement", $"Invoice {invoice.InvoiceNumber} marked as externally managed");
            return (true, "Invoice marked as externally managed");
        }

        if (invoice.CollectionMode == InvoiceCollectionMode.ShadowOnly)
        {
            invoice.Status = InvoiceStatus.ShadowOnly;
            await db.SaveChangesAsync();
            await audit.LogAsync(actorEmail, "Invoice", invoiceId, "ShadowBilling", $"Invoice {invoice.InvoiceNumber} shadow billed only");
            return (true, "Shadow billing — no payment collected");
        }

        if (invoice.CollectionMode == InvoiceCollectionMode.NoCharge)
        {
            invoice.Status = InvoiceStatus.Paid;
            await db.SaveChangesAsync();
            return (true, "No charge — invoice marked paid");
        }

        // Determine payment method from booking's travel policy
        SavedPaymentMethod? pm = null;
        if (invoice.Booking?.TravelPolicy?.SavedPaymentMethod != null)
            pm = invoice.Booking.TravelPolicy.SavedPaymentMethod;
        else
        {
            // Fall back to default payment method for buyer org
            var profile = await db.BuyerPaymentProfiles
                .Include(x => x.SavedPaymentMethods)
                .FirstOrDefaultAsync(x => x.BuyerOrganisationId == invoice.BuyerOrganisationId);
            pm = profile?.SavedPaymentMethods.FirstOrDefault(x => x.IsDefault)
              ?? profile?.SavedPaymentMethods.FirstOrDefault();
        }

        int counter;
        lock (_lock) { counter = _piCounter++; }

        var fakePaymentId = $"pi_mock_{counter:D6}";

        // Simulate outcome by card last4
        bool succeeds = pm?.Last4 switch
        {
            "1111" => invoice.PaymentAttempts.Count > 0, // fails first, succeeds on retry
            _ => true
        };

        var attempt = new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            InternalInvoiceId = invoiceId,
            AttemptedAtUtc = DateTimeOffset.UtcNow,
            ProviderType = invoice.PaymentProviderType,
            FakeProviderPaymentId = fakePaymentId,
            SavedPaymentMethodId = pm?.Id,
            Succeeded = succeeds,
            FailureReason = succeeds ? null : "insufficient_funds (simulated)",
            Amount = invoice.TotalAmount,
            CurrencyCode = invoice.CurrencyCode
        };
        db.PaymentAttempts.Add(attempt);

        var ledgerType = succeeds ? LedgerEntryType.PaymentCaptured : LedgerEntryType.PaymentFailed;
        db.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            SellerOrganisationId = invoice.SellerOrganisationId,
            BuyerOrganisationId = invoice.BuyerOrganisationId,
            BookingId = invoice.BookingId,
            InternalInvoiceId = invoiceId,
            EntryType = ledgerType,
            Description = succeeds ? $"Payment captured via {invoice.PaymentProviderType} [{fakePaymentId}]" : $"Payment failed via {invoice.PaymentProviderType} — {attempt.FailureReason}",
            DebitAmount = succeeds ? 0 : invoice.TotalAmount,
            CreditAmount = succeeds ? invoice.TotalAmount : 0,
            CurrencyCode = invoice.CurrencyCode,
            ExternalReference = fakePaymentId
        });

        invoice.Status = succeeds ? InvoiceStatus.Paid : InvoiceStatus.Failed;
        if (succeeds) invoice.FakeExternalInvoiceId = $"in_mock_{counter:D6}";

        await db.SaveChangesAsync();

        var action = succeeds ? "PaymentSimulated" : "PaymentFailed";
        await audit.LogAsync(actorEmail, "Invoice", invoiceId, action,
            $"Invoice {invoice.InvoiceNumber}: {(succeeds ? "payment succeeded" : "payment failed")} via {invoice.PaymentProviderType}. Provider action simulated only. No external payment was made.");

        return (succeeds, succeeds ? "Payment succeeded (simulated)" : "Payment failed (simulated)");
    }
}
