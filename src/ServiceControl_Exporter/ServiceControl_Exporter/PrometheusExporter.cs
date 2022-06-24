namespace ServiceControl_Exporter;

using Config;
using Microsoft.Extensions.Hosting;
using Prometheus;

internal class PrometheusExporter : IHostedService
{
    private readonly KestrelMetricServer _server;

    public PrometheusExporter(AppSettings configuration)
    {
        _server = new KestrelMetricServer(configuration.Exporter.Host,
                                          configuration.Exporter.Port);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _server.Start();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _server.StopAsync();
    }
}