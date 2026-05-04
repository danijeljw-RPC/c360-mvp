namespace Cinturon360.Mock.Api.Data.Entities;
public sealed class ImportedContentBatch
{
    public Guid Id { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ImportType { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";
    public int RecordCount { get; set; }
    public string ImportedByEmail { get; set; } = string.Empty;
    public DateTimeOffset ImportedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
