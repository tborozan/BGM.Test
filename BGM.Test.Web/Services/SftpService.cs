using System.Xml;
using BGM.Test.Web.DAL;
using BGM.Test.Web.Helpers;
using BGM.Test.Web.Mappers;
using BGM.Test.Web.Models.Configuration;
using BGM.Test.Web.Models.Documents;
using BGM.Test.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace BGM.Test.Web.Services;

public sealed class SftpService : ISftpService, IDisposable
{
    private readonly BgmDbContext _bgmDbContext;
    private readonly SftpClient _sftpClient;

    public SftpService(IOptions<SftpOptions> sftpOptions, BgmDbContext bgmDbContext)
    {
        _bgmDbContext = bgmDbContext;
        _sftpClient = new SftpClient(sftpOptions.Value.Host, sftpOptions.Value.Port, sftpOptions.Value.Username, sftpOptions.Value.Password);
    }

    public async Task<int> ImportInvoices(CancellationToken cancellationToken = default)
    {
        if (!_sftpClient.IsConnected)
            await _sftpClient.ConnectAsync(cancellationToken);

        IEnumerable<ISftpFile>? dir = _sftpClient.ListDirectory("inbox");

        foreach (ISftpFile file in dir.Where(x => Path.GetExtension(x.Name) == ".xml"))
        {
            bool exists = await _bgmDbContext.Invoices.AnyAsync(x => x.FileName == file.Name, cancellationToken: cancellationToken);
            if (exists)
                continue;

            StreamReader? reader = _sftpClient.OpenText(file.FullName);
            Invoice? document = XmlSerializer<Invoice>.Deserialize(reader);
            if (document is null)
                continue;
            
            DAL.Models.Invoice? invoice = InvoiceMapper.MapToEntity(document, file.Name);
            _bgmDbContext.Add(invoice);
        }

        return await _bgmDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ExportPaidInvoices(CancellationToken cancellationToken = default)
    {
        if (!_sftpClient.IsConnected)
            await _sftpClient.ConnectAsync(cancellationToken);

        List<Invoice> invoices = await _bgmDbContext.Invoices
            .Where(x => x.IsPaid)
            .Select(x => new Invoice()
                {
                    Title = x.Title,
                }
            )
            .ToListAsync(cancellationToken: cancellationToken);

        var report = new Report()
        {
            PaidInvoices = invoices,
        };
        var reportName = $"outbox/report_{DateOnly.FromDateTime(DateTime.Today).ToString("o")}.xml";

        bool exists = _sftpClient.Exists(reportName);
        if (exists)
            return;

        await CreateFile(report, reportName);
    }

    private async Task CreateFile<T>(T obj, string fileName) where T : class
    {
        await using SftpFileStream? fileStream = _sftpClient.OpenWrite(fileName);
        await using var streamWriter = new StreamWriter(fileStream);
        await using var writer = XmlWriter.Create(streamWriter, settings: new XmlWriterSettings { Async = true });
        XmlSerializer<T>.Serialize(writer, obj);
    }

    public void Dispose()
    {
        if (_sftpClient.IsConnected)
            _sftpClient.Disconnect();

        _sftpClient.Dispose();
    }
}