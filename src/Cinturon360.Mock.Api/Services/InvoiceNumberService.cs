using Cinturon360.Mock.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinturon360.Mock.Api.Services;

public sealed class InvoiceNumberService(AppDbContext db)
{
    public async Task<string> NextAsync()
    {
        var count = await db.InternalInvoices.CountAsync();
        return $"INV-HWT-{(count + 1):D4}";
    }
}
