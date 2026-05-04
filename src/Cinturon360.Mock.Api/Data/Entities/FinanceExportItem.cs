namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class FinanceExportItem
{
    public Guid Id { get; set; }
    public Guid FinanceExportBatchId { get; set; }
    public FinanceExportBatch? FinanceExportBatch { get; set; }
    public Guid InternalInvoiceId { get; set; }
    public InternalInvoice? InternalInvoice { get; set; }
    public DateTimeOffset ExportedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
