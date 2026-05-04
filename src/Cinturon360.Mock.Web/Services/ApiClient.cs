using System.Net.Http.Json;
using Cinturon360.Mock.Shared.Dtos;

namespace Cinturon360.Mock.Web.Services;

public sealed class ApiClient(HttpClient http, SessionState session)
{
    private void SetActor()
    {
        if (!string.IsNullOrEmpty(session.UserEmail))
        {
            http.DefaultRequestHeaders.Remove("X-Actor-Email");
            http.DefaultRequestHeaders.Add("X-Actor-Email", session.UserEmail);
        }
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest req)
    {
        var resp = await http.PostAsJsonAsync("api/auth/login", req);
        return await resp.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<DashboardDto?> GetDashboardAsync() { SetActor(); return await http.GetFromJsonAsync<DashboardDto>("api/dashboard"); }
    public async Task<List<OrganisationDto>?> GetOrgTreeAsync() { SetActor(); return await http.GetFromJsonAsync<List<OrganisationDto>>("api/organisations/tree"); }
    public async Task<List<BillingRelationshipDto>?> GetBillingRelationshipsAsync() => await http.GetFromJsonAsync<List<BillingRelationshipDto>>("api/billing-relationships");
    public async Task<List<PaymentMethodDto>?> GetPaymentMethodsAsync() => await http.GetFromJsonAsync<List<PaymentMethodDto>>("api/payment-methods");
    public async Task<List<TravelPolicyDto>?> GetTravelPoliciesAsync() => await http.GetFromJsonAsync<List<TravelPolicyDto>>("api/travel-policies");

    public async Task<List<BookingDto>?> GetBookingsAsync(string? status = null, string? pnr = null)
    {
        SetActor();
        var qs = new List<string>();
        if (!string.IsNullOrEmpty(status)) qs.Add($"status={status}");
        if (!string.IsNullOrEmpty(pnr)) qs.Add($"pnr={pnr}");
        var url = "api/bookings" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
        return await http.GetFromJsonAsync<List<BookingDto>>(url);
    }

    public async Task<BookingDto?> GetBookingAsync(Guid id) { SetActor(); return await http.GetFromJsonAsync<BookingDto>($"api/bookings/{id}"); }
    public async Task MarkReadyForBillingAsync(Guid id) { SetActor(); await http.PostAsync($"api/bookings/{id}/mark-ready-for-billing", null); }

    public async Task<InvoiceDto?> GenerateInvoiceAsync(Guid bookingId)
    {
        SetActor();
        var r = await http.PostAsync($"api/bookings/{bookingId}/generate-invoice", null);
        return await r.Content.ReadFromJsonAsync<InvoiceDto>();
    }

    public async Task SetCostCentreAsync(Guid bookingId, string code)
    {
        SetActor();
        await http.PostAsJsonAsync($"api/bookings/{bookingId}/set-cost-centre", new SetCostCentreRequest { CostCentreCode = code });
    }

    public async Task SetTravelPolicyAsync(Guid bookingId, Guid policyId)
    {
        SetActor();
        await http.PostAsJsonAsync($"api/bookings/{bookingId}/set-travel-policy", new SetTravelPolicyRequest { TravelPolicyId = policyId });
    }

    public async Task<List<ImportFileDto>?> GetGdsFilesAsync() => await http.GetFromJsonAsync<List<ImportFileDto>>("api/import-files/gds");

    public async Task<GdsImportResultDto?> ImportGdsFileAsync(string fileName)
    {
        SetActor();
        var r = await http.PostAsync($"api/imports/gds/{fileName}", null);
        return await r.Content.ReadFromJsonAsync<GdsImportResultDto>();
    }

    public async Task<List<InvoiceDto>?> GetInvoicesAsync() { SetActor(); return await http.GetFromJsonAsync<List<InvoiceDto>>("api/invoices"); }
    public async Task<InvoiceDto?> GetInvoiceAsync(Guid id) { SetActor(); return await http.GetFromJsonAsync<InvoiceDto>($"api/invoices/{id}"); }

    public async Task<(bool succeeded, string message)> SimulateCollectionAsync(Guid invoiceId)
    {
        SetActor();
        var r = await http.PostAsync($"api/invoices/{invoiceId}/simulate-collection", null);
        var d = await r.Content.ReadFromJsonAsync<SimulationResult>();
        return (d?.succeeded ?? false, d?.message ?? "");
    }

    public async Task<(bool succeeded, string message)> RetryPaymentAsync(Guid invoiceId)
    {
        SetActor();
        var r = await http.PostAsync($"api/invoices/{invoiceId}/retry-payment", null);
        var d = await r.Content.ReadFromJsonAsync<SimulationResult>();
        return (d?.succeeded ?? false, d?.message ?? "");
    }

    public async Task MarkManualPaymentReceivedAsync(Guid invoiceId) { SetActor(); await http.PostAsync($"api/invoices/{invoiceId}/mark-manual-payment-received", null); }
    public async Task MarkExternallySettledAsync(Guid invoiceId) { SetActor(); await http.PostAsync($"api/invoices/{invoiceId}/mark-externally-settled", null); }

    public async Task<List<AgencyMemoDto>?> GetAgencyMemosAsync() { SetActor(); return await http.GetFromJsonAsync<List<AgencyMemoDto>>("api/agency-memos"); }
    public async Task<AgencyMemoDto?> GetAgencyMemoAsync(Guid id) { SetActor(); return await http.GetFromJsonAsync<AgencyMemoDto>($"api/agency-memos/{id}"); }

    public async Task SetMemoOutcomeAsync(Guid id, Cinturon360.Mock.Shared.Enums.AgencyMemoOutcome outcome)
    {
        SetActor();
        await http.PostAsJsonAsync($"api/agency-memos/{id}/set-outcome", new SetMemoOutcomeRequest { Outcome = outcome });
    }

    public async Task<object?> ImportBspMemosAsync(string csv)
    {
        SetActor();
        var r = await http.PostAsJsonAsync("api/agency-memos/import-csv", csv);
        return await r.Content.ReadFromJsonAsync<object>();
    }

    public async Task<List<LedgerEntryDto>?> GetLedgerAsync(Guid? bookingId = null, Guid? invoiceId = null)
    {
        SetActor();
        var qs = new List<string>();
        if (bookingId.HasValue) qs.Add($"bookingId={bookingId}");
        if (invoiceId.HasValue) qs.Add($"invoiceId={invoiceId}");
        return await http.GetFromJsonAsync<List<LedgerEntryDto>>("api/ledger" + (qs.Count > 0 ? "?" + string.Join("&", qs) : ""));
    }

    public async Task<List<AuditEventDto>?> GetAuditAsync(string? entityType = null, Guid? entityId = null)
    {
        SetActor();
        var qs = new List<string>();
        if (!string.IsNullOrEmpty(entityType)) qs.Add($"entityType={entityType}");
        if (entityId.HasValue) qs.Add($"entityId={entityId}");
        return await http.GetFromJsonAsync<List<AuditEventDto>>("api/audit" + (qs.Count > 0 ? "?" + string.Join("&", qs) : ""));
    }

    public async Task<List<OutstandingBalanceReportItem>?> GetOutstandingBalancesAsync() => await http.GetFromJsonAsync<List<OutstandingBalanceReportItem>>("api/reports/outstanding-balances");
    public async Task<List<AdmAcmExposureItem>?> GetAdmAcmExposureAsync() => await http.GetFromJsonAsync<List<AdmAcmExposureItem>>("api/reports/adm-acm-exposure");

    public async Task<List<FinanceExportBatchDto>?> GetFinanceExportsAsync() => await http.GetFromJsonAsync<List<FinanceExportBatchDto>>("api/finance-exports");

    public async Task<FinanceExportBatchDto?> CreateFinanceExportAsync(string target)
    {
        SetActor();
        var r = await http.PostAsJsonAsync("api/finance-exports", new FinanceExportRequestDto { ExportTarget = target });
        return await r.Content.ReadFromJsonAsync<FinanceExportBatchDto>();
    }

    public string GetExportDownloadUrl(Guid batchId) => $"{http.BaseAddress}api/finance-exports/{batchId}/download";

    private sealed record SimulationResult(bool succeeded, string message, string note);
}
