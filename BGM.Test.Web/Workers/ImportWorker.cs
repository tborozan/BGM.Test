using BGM.Test.Web.Models.Configuration;
using BGM.Test.Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BGM.Test.Web.Workers;

public sealed class ImportWorker : IHostedService, IDisposable, IAsyncDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer? _timer;

    public ImportWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var workerOptions = scope.ServiceProvider.GetRequiredService<IOptions<ImportWorkerOptions>>();
        _timer = new Timer(ImportDocuments, null, TimeSpan.FromSeconds(10), workerOptions.Value.Interval);

        return Task.CompletedTask;
    }

    private async void ImportDocuments(object? state)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ImportWorker>>();
        try
        {
            var sftpService = scope.ServiceProvider.GetRequiredService<ISftpService>();
            int importedCount = await sftpService.ImportInvoices();
            logger.LogInformation("Imported {ImportCount} documents", importedCount);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer is not null)
            await _timer.DisposeAsync();
    }
}