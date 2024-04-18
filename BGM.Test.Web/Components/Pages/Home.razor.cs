using BGM.Test.Web.DAL.Models;
using BGM.Test.Web.Mappers;
using BGM.Test.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BGM.Test.Web.Components.Pages;

public sealed partial class Home : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    public ICollection<InvoiceVm> Invoices { get; set; } = new List<InvoiceVm>();

    protected override async Task OnInitializedAsync()
    {
        await LoadInvoices();
    }

    private async Task LoadInvoices()
    {
        List<Invoice>? invoices = await DbContext.Invoices.ToListAsync();
        List<InvoiceVm>? invoiceVms = invoices.Select(InvoiceMapper.MapToViewModel).ToList();

        Invoices = invoiceVms;
    }

    private async Task ImportInvoices()
    {
        await SftpService.ImportInvoices(_cts.Token);

        await LoadInvoices();
    }

    public void Dispose()
    {
        if (!_cts.IsCancellationRequested)
            _cts.Cancel();

        _cts.Dispose();
    }

    private async Task ExportInvoices()
    {
        await SftpService.ExportPaidInvoices(_cts.Token);
    }

    private async Task MarkAllAsPaid()
    {
        foreach (InvoiceVm? invoiceVm in Invoices.Where(x => !x.IsPaid))
        {
            Invoice? invoice = DbContext.Invoices.FirstOrDefault(x => x.Id == invoiceVm.Id);
            if (invoice is null)
                continue;

            invoice.IsPaid = true;
        }

        await DbContext.SaveChangesAsync();
        await LoadInvoices();
    }
}