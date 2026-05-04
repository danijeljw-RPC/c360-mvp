using Cinturon360.Mock.Api.Services;
using Cinturon360.Mock.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Cinturon360.Mock.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class ImportsController(GdsImportService gdsImport) : ControllerBase
{
    private static readonly string GdsMockDataPath =
        Path.Combine(AppContext.BaseDirectory, "mock-data", "gds");

    [HttpGet("import-files/gds")]
    public ActionResult<List<ImportFileDto>> ListGdsFiles()
    {
        if (!Directory.Exists(GdsMockDataPath))
            return Ok(new List<ImportFileDto>());

        var files = Directory.GetFiles(GdsMockDataPath, "*.json")
            .Select(f => new ImportFileDto
            {
                FileName = Path.GetFileName(f),
                DisplayName = Path.GetFileNameWithoutExtension(f).Replace("-", " "),
                Description = "Mock GDS PNR import file"
            }).ToList();
        return Ok(files);
    }

    [HttpPost("imports/gds/{fileName}")]
    public async Task<ActionResult<GdsImportResultDto>> ImportGds(string fileName, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        var path = Path.Combine(GdsMockDataPath, fileName);
        if (!System.IO.File.Exists(path))
            return NotFound($"File {fileName} not found");

        var content = await System.IO.File.ReadAllTextAsync(path);
        var result = await gdsImport.ImportFileAsync(content, fileName, actorEmail ?? "system");
        return Ok(result);
    }

    [HttpPost("imports/bsp/agency-memos")]
    public async Task<ActionResult> ImportBspMemos([FromBody] string csvContent, [FromHeader(Name = "X-Actor-Email")] string? actorEmail)
    {
        return Ok(new { message = "BSP memo import accepted. Use the ADM/ACM workbench to view imported records." });
    }
}
