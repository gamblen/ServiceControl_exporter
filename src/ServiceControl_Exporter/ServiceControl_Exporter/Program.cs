namespace ServiceControl_Exporter;

using System.Net;
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

        FlurlHttp.Clients.WithDefaults(settings =>
                                       {
                                           settings.ConfigureInnerHandler(handler =>
                                                                          {
                                                                              if (!string.IsNullOrWhiteSpace(appSettings.ProxyUrl))
                                                                              {
                                                                                  handler.Proxy = new WebProxy(new Uri(appSettings.ProxyUrl));
                                                                                  handler.UseProxy = true;
                                                                              }
                                                                          });
                                       });

        Metrics.DefaultRegistry.AddBeforeCollectCallback(async cancel =>
                                                         {
                                                             await mediator.Send(new UpdateEndpointStatsMetricsCommand(), cancel).ConfigureAwait(false);
                                                             await mediator.Send(new UpdateErrorMetricsCommand(), cancel).ConfigureAwait(false);
                                                             await mediator.Send(new UpdateCustomChecksMetricsCommand(), cancel).ConfigureAwait(false);
                                                             await mediator.Send(new UpdateHeartBeatStatsMetricsCommand(), cancel).ConfigureAwait(false);
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
                                          c.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
                                      })
                   .ConfigureLogging(c => c.AddConsole());
    }
}