using System.Net;
using Flurl.Http;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using ServiceControl_Exporter;
using ServiceControl_Exporter.Commands;
using ServiceControl_Exporter.Config;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(provider => provider.GetRequiredService<IConfiguration>().Get<AppSettings>());
builder.Services.AddSingleton<CollectorDictionary>();
builder.Services.AddHostedService<PrometheusExporter>();
builder.Services.AddMediator();
builder.Logging.AddConsole();

var appSettings = builder.Configuration.Get<AppSettings>();

var registry = Metrics.DefaultRegistry;

var serverOptions = new KestrelMetricServerOptions
                    {
                        Registry = registry,
                        Hostname = appSettings.Exporter.Host,
                        Port = appSettings.Exporter.Port
                    };
builder.Services.AddSingleton(serverOptions);

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

var host = builder.Build();

var mediator = host.Services.GetRequiredService<IMediator>();

registry.AddBeforeCollectCallback(async cancel =>
                                  {
                                      try
                                      {
                                          var logger = host.Services.GetRequiredService<ILogger<PrometheusExporter>>();
                                          // If you see this in your container logs, the network route is working
                                          if (logger.IsEnabled(LogLevel.Debug))
                                              logger.Log(LogLevel.Debug, "Kestrel handed execution to AddBeforeCollectCallback.");

                                          await mediator.Send(new UpdateEndpointStatsMetricsCommand(), cancel).ConfigureAwait(false);
                                          await mediator.Send(new UpdateErrorMetricsCommand(), cancel).ConfigureAwait(false);
                                          await mediator.Send(new UpdateCustomChecksMetricsCommand(), cancel).ConfigureAwait(false);
                                          await mediator.Send(new UpdateHeartBeatStatsMetricsCommand(), cancel).ConfigureAwait(false);
                                          await mediator.Send(new UpdateMonitoringMetricsCommand(), cancel).ConfigureAwait(false);

                                          if (logger.IsEnabled(LogLevel.Debug))
                                              logger.Log(LogLevel.Debug, "Callback completed successfully.");
                                      }
                                      catch (Exception ex)
                                      {
                                          var logger = host.Services.GetRequiredService<ILogger<PrometheusExporter>>();
                                          // Kestrel swallows pipeline faults; this forces it out to your container log stream
                                          if (logger.IsEnabled(LogLevel.Error))
                                              logger.Log(LogLevel.Error, ex, "An error occurred while collecting metrics.");
                                          throw; // Allow prometheus-net to handle the 503 response
                                      }
                                  });

host.Run();