namespace Cinturon360.Mock.Shared.Dtos;
public sealed class FinanceExportRequestDto
{
    public string ExportTarget { get; set; } = "Xero";
    public List<Guid> InvoiceIds { get; set; } = [];
}
public sealed class FinanceExportBatchDto
{
    public Guid Id { get; set; }
    public string ExportTarget { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public int InvoiceCount { get; set; }
    public string Status { get; set; } = string.Empty;
}
