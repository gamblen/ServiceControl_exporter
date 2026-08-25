namespace ServiceControl_Exporter;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

internal class PrometheusExporter(KestrelMetricServerOptions options, ILogger<PrometheusExporter> logger) : IHostedService
{
    private readonly KestrelMetricServer _server = new(options);

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Prometheus exporter for {0}:{1}", options.Hostname, options.Port);
        _server.Start();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Prometheus exporter for {0}:{1}", options.Hostname, options.Port);
        await _server.StopAsync();
    }
}