using System.Text.Json;
using Cinturon360.Mock.Api.Data;
using Cinturon360.Mock.Api.Data.Entities;
using Cinturon360.Mock.Shared.Dtos;
using Cinturon360.Mock.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Services;

public sealed class GdsImportService(AppDbContext db, AuditService audit)
{
    public async Task<GdsImportResultDto> ImportFileAsync(string fileContent, string fileName, string actorEmail)
    {
        using var doc = JsonDocument.Parse(fileContent);
        var root = doc.RootElement;

        var pnr = root.GetProperty("pnr").GetString() ?? "";
        var source = root.TryGetProperty("source", out var src) ? src.GetString() ?? "MockGDS" : "MockGDS";
        var clientCode = root.TryGetProperty("client", out var cl) ? cl.GetString() ?? "" : "";
        var policyCode = root.TryGetProperty("policyCode", out var pc) ? pc.GetString() : null;
        var costCentre = root.TryGetProperty("costCentre", out var cc) ? cc.GetString() : null;
        var scenario = root.TryGetProperty("paymentScenario", out var sc) ? sc.GetString() ?? "" : "IMMEDIATE_SUCCESS";

        // Resolve orgs
        var clientOrg = await db.Organisations.FirstOrDefaultAsync(o => o.Code == "COCACOLA");
        var tmcOrg = await db.Organisations.FirstOrDefaultAsync(o => o.Code == "HELLOWORLD");
        var vendorOrg = await db.Organisations.FirstOrDefaultAsync(o => o.Code == "AVANOA");

        if (clientOrg == null || tmcOrg == null || vendorOrg == null)
            return new GdsImportResultDto { Success = false, ErrorMessage = "Seed organisations missing" };

        // Check for duplicate
        if (await db.Bookings.AnyAsync(b => b.PnrCode == pnr))
            return new GdsImportResultDto { Success = false, ErrorMessage = $"PNR {pnr} already imported" };

        // Traveller
        var travellerEl = root.GetProperty("traveller");
        var travellerEmail = travellerEl.GetProperty("email").GetString() ?? "";
        var traveller = await db.Travellers.FirstOrDefaultAsync(t => t.Email == travellerEmail);
        if (traveller == null)
        {
            var fullName = travellerEl.GetProperty("name").GetString() ?? "Unknown";
            var parts = fullName.Split(' ', 2);
            traveller = new Traveller
            {
                Id = Guid.NewGuid(),
                ClientOrganisationId = clientOrg.Id,
                FirstName = parts[0],
                LastName = parts.Length > 1 ? parts[1] : "",
                Email = travellerEmail,
                DefaultCostCentreCode = costCentre
            };
            db.Travellers.Add(traveller);
        }

        // Travel policy
        TravelPolicy? policy = policyCode != null
            ? await db.TravelPolicies.Include(p => p.SavedPaymentMethod).FirstOrDefaultAsync(p => p.Code == policyCode)
            : null;

        // Amounts
        decimal fare = 0, tax = 0, serviceFee = 0, surcharge = 0;
        DateOnly departure = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60));
        DateOnly returnDate = departure.AddDays(7);

        if (root.TryGetProperty("ticket", out var ticket))
        {
            fare = ticket.GetProperty("fare").GetDecimal();
            tax = ticket.GetProperty("tax").GetDecimal();
        }
        if (root.TryGetProperty("package", out var pkg))
        {
            fare = pkg.GetProperty("total").GetDecimal();
            serviceFee = pkg.TryGetProperty("serviceFee", out var sf) ? sf.GetDecimal() : 0;
            if (pkg.TryGetProperty("departureDate", out var dd))
                departure = DateOnly.Parse(dd.GetString()!);
        }
        if (root.TryGetProperty("serviceFees", out var fees))
            foreach (var f in fees.EnumerateArray())
                serviceFee += f.GetProperty("amount").GetDecimal();

        // Surcharge for card payments
        if (scenario == "IMMEDIATE_SUCCESS" || scenario == "PAYMENT_FAIL_THEN_RETRY")
            surcharge = Math.Round((fare + tax) * 0.015m, 2);

        var total = fare + tax + serviceFee + surcharge;

        // Segments
        var segments = new List<BookingSegment>();
        if (root.TryGetProperty("segments", out var segsEl))
        {
            foreach (var seg in segsEl.EnumerateArray())
            {
                var fromLoc = seg.TryGetProperty("from", out var fr) ? fr.GetString() : null;
                var toLoc = seg.TryGetProperty("to", out var to) ? to.GetString() : null;
                DateTimeOffset? depUtc = null, arrUtc = null;
                if (seg.TryGetProperty("depart", out var dep)) depUtc = DateTimeOffset.Parse(dep.GetString()!).ToUniversalTime();
                if (seg.TryGetProperty("arrive", out var arr)) arrUtc = DateTimeOffset.Parse(arr.GetString()!).ToUniversalTime();
                if (depUtc.HasValue) departure = DateOnly.FromDateTime(depUtc.Value.DateTime);

                segments.Add(new BookingSegment
                {
                    Id = Guid.NewGuid(),
                    SegmentType = seg.TryGetProperty("type", out var st) ? st.GetString() ?? "AIR" : "AIR",
                    Carrier = seg.TryGetProperty("carrier", out var car) ? car.GetString() : null,
                    FlightNumber = seg.TryGetProperty("flight", out var fl) ? fl.GetString() : null,
                    FromLocation = fromLoc,
                    ToLocation = toLoc,
                    DepartureUtc = depUtc,
                    ArrivalUtc = arrUtc
                });
            }
        }
        returnDate = departure.AddDays(7);

        // Booking status
        var missing = new List<string>();
        if (policy == null) missing.Add("TravelPolicy");
        if (string.IsNullOrEmpty(costCentre)) missing.Add("CostCentreCode");
        if (fare == 0) missing.Add("SupplierAmount");

        var status = missing.Count > 0 ? BookingStatus.NeedsReview : BookingStatus.ReadyForBilling;

        var ticketNumber = root.TryGetProperty("ticket", out var t2) && t2.TryGetProperty("number", out var tn)
            ? tn.GetString() : null;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingReference = $"BKG-{pnr}",
            PnrCode = pnr,
            TicketNumber = ticketNumber,
            TravellerId = traveller.Id,
            ClientOrganisationId = clientOrg.Id,
            TmcOrganisationId = tmcOrg.Id,
            VendorOrganisationId = vendorOrg.Id,
            TravelPolicyId = policy?.Id,
            CostCentreCode = costCentre,
            Status = status,
            DepartureDate = departure,
            ReturnDate = returnDate,
            CurrencyCode = "AUD",
            SupplierAmount = fare,
            TaxAmount = tax,
            ServiceFeeAmount = serviceFee,
            SurchargeAmount = surcharge,
            TotalAmount = total,
            SourceSystem = source
        };
        booking.Segments.AddRange(segments);

        // Payment schedule for deposit scenario
        if (scenario == "DEPOSIT_NOW_BALANCE_LATER" && root.TryGetProperty("package", out var pkgSched))
        {
            var deposit = pkgSched.GetProperty("deposit").GetDecimal();
            var balance = pkgSched.GetProperty("balance").GetDecimal();
            var balanceDueDays = pkgSched.TryGetProperty("balanceDueDaysBeforeDeparture", out var bdd) ? bdd.GetInt32() : 30;
            var balanceDue = departure.AddDays(-balanceDueDays);

            var schedule = new BookingPaymentSchedule
            {
                Id = Guid.NewGuid(),
                TotalScheduledAmount = total,
                CurrencyCode = "AUD",
                Status = PaymentScheduleStatus.Pending
            };
            schedule.Items.Add(new BookingPaymentScheduleItem { Id = Guid.NewGuid(), ItemType = PaymentScheduleItemType.Deposit, PaymentTiming = PaymentTiming.Immediate, Description = "Deposit payment", Amount = deposit, Status = PaymentScheduleItemStatus.Pending });
            schedule.Items.Add(new BookingPaymentScheduleItem { Id = Guid.NewGuid(), ItemType = PaymentScheduleItemType.ServiceFee, PaymentTiming = PaymentTiming.Immediate, Description = "Booking/service fee", Amount = serviceFee, Status = PaymentScheduleItemStatus.Pending });
            schedule.Items.Add(new BookingPaymentScheduleItem { Id = Guid.NewGuid(), ItemType = PaymentScheduleItemType.SupplierBalance, PaymentTiming = PaymentTiming.DueOnDate, Description = "Supplier balance", Amount = balance, DueDate = balanceDue, Status = PaymentScheduleItemStatus.Pending });
            booking.PaymentSchedules.Add(schedule);
        }
        else
        {
            var schedule = new BookingPaymentSchedule
            {
                Id = Guid.NewGuid(),
                TotalScheduledAmount = total,
                CurrencyCode = "AUD",
                Status = PaymentScheduleStatus.Pending
            };
            if (fare > 0) schedule.Items.Add(new BookingPaymentScheduleItem { Id = Guid.NewGuid(), ItemType = PaymentScheduleItemType.SupplierBalance, PaymentTiming = PaymentTiming.Immediate, Description = "Supplier fare", Amount = fare, Status = PaymentScheduleItemStatus.Pending });
            if (tax > 0) schedule.Items.Add(new BookingPaymentScheduleItem { Id = Guid.NewGuid(), ItemType = PaymentScheduleItemType.Tax, PaymentTiming = PaymentTiming.Immediate, Description = "Taxes and charges", Amount = tax, Status = PaymentScheduleItemStatus.Pending });
            if (serviceFee > 0) schedule.Items.Add(new BookingPaymentScheduleItem { Id = Guid.NewGuid(), ItemType = PaymentScheduleItemType.ServiceFee, PaymentTiming = PaymentTiming.Immediate, Description = "TMC booking fee", Amount = serviceFee, Status = PaymentScheduleItemStatus.Pending });
            if (surcharge > 0) schedule.Items.Add(new BookingPaymentScheduleItem { Id = Guid.NewGuid(), ItemType = PaymentScheduleItemType.CardSurcharge, PaymentTiming = PaymentTiming.Immediate, Description = "Card surcharge", Amount = surcharge, Status = PaymentScheduleItemStatus.Pending });
            booking.PaymentSchedules.Add(schedule);
        }

        db.Bookings.Add(booking);

        db.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            SellerOrganisationId = tmcOrg.Id,
            BuyerOrganisationId = clientOrg.Id,
            BookingId = booking.Id,
            EntryType = LedgerEntryType.SupplierPayableRecognised,
            Description = $"Booking {booking.BookingReference} imported from {source}",
            DebitAmount = total,
            CreditAmount = 0,
            CurrencyCode = "AUD"
        });

        db.ImportedContentBatches.Add(new ImportedContentBatch
        {
            Id = Guid.NewGuid(),
            SourceSystem = source,
            FileName = fileName,
            ImportType = "GDS",
            Status = "Completed",
            RecordCount = 1,
            ImportedByEmail = actorEmail
        });

        await db.SaveChangesAsync();

        await audit.LogAsync(actorEmail, "Booking", booking.Id, "GdsImportCompleted",
            $"PNR {pnr} imported. Status: {status}. Missing: {(missing.Count > 0 ? string.Join(", ", missing) : "none")}");

        return new GdsImportResultDto
        {
            Success = true,
            PnrCode = pnr,
            BookingReference = booking.BookingReference,
            BookingId = booking.Id,
            MissingFields = missing,
            NeedsReview = missing.Count > 0
        };
    }
}
