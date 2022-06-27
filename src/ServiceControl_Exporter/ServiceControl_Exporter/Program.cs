namespace ServiceControl_Exporter;

using Commands;
using Config;
using Flurl.Http;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

internal class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();

        var mediator = host.Services.GetRequiredService<IMediator>();

        var appSettings = host.Services.GetRequiredService<AppSettings>();

        FlurlHttp.Configure(settings =>
                            {
                                settings.HttpClientFactory = new ProxyHttpClientFactory(appSettings.ProxyUrl);
                            });

        Metrics.DefaultRegistry.AddBeforeCollectCallback(async cancel =>
                                                         {
                                                             await mediator.Send(new UpdateErrorMetricsCommand(), cancel).ConfigureAwait(false);
                                                             await mediator.Send(new UpdateCustomChecksMetricsCommand(), cancel).ConfigureAwait(false);
                                                             await mediator.Send(new UpdateHeartBeatMetricsCommand(), cancel).ConfigureAwait(false);
                                                             await mediator.Send(new UpdateMonitoringMetricsCommand(), cancel).ConfigureAwait(false);
                                                         });

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .UseWindowsService()
                   .UseSystemd()
                   .ConfigureServices(c =>
                                      {
                                          c.AddSingleton(provider => provider.GetRequiredService<IConfiguration>().Get<AppSettings>());
                                          c.AddSingleton<CollectorDictionary>();
                                          c.AddHostedService<PrometheusExporter>();
                                          c.AddMediatR(typeof(Program));
                                      })
                   .ConfigureLogging(c => c.AddConsole());
    }
}