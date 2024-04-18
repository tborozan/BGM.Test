namespace BGM.Test.Web.Services.Interfaces;

public interface ISftpService
{
    Task<int> ImportInvoices(CancellationToken cancellationToken = default);
    Task ExportPaidInvoices(CancellationToken cancellationToken = default);
}