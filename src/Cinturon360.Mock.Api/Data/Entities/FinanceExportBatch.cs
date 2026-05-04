namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class FinanceExportBatch
{
    public Guid Id { get; set; }
    public string ExportTarget { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedByEmail { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public string Status { get; set; } = "Completed";
    public List<FinanceExportItem> Items { get; set; } = [];
}
