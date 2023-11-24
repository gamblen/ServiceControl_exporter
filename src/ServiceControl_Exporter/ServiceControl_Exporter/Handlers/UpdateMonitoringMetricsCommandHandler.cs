namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Models.Metrics;
using Prometheus;
using Metrics = Prometheus.Metrics;

public class UpdateMonitoringMetricsCommandHandler : IRequestHandler<UpdateMonitoringMetricsCommand>
{
    private readonly CollectorDictionary _metrics;
    private readonly AppSettings _configuration;

    public UpdateMonitoringMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration)
    {
        _metrics = metrics;
        _configuration = configuration;
    }

    public async Task Handle(UpdateMonitoringMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_configuration.Include.Monitoring ||
                _configuration.ServiceControl.MonitoringUrls == null ||
                !_configuration.ServiceControl.MonitoringUrls.Any())
                return;

            if (!_metrics.ContainsKey("servicecontrol_monitoring_endpoints"))
                _metrics.Add("servicecontrol_monitoring_endpoints", Metrics.CreateGauge("servicecontrol_monitoring_endpoints", "monitoring endpoints", "endpoint", "metric"));

            foreach (var monitoringUrl in _configuration.ServiceControl.MonitoringUrls)
            {
                try
                {
                    var url = Url.Combine(monitoringUrl, "monitored-endpoints");


                    var response = await url.SetQueryParam("history", "1").GetJsonAsync<IList<Root>>(cancellationToken).ConfigureAwait(false);

                    if (_metrics["servicecontrol_monitoring_endpoints"] is Gauge gauge)
                    {
                        foreach (var item in response)
                        {
                            gauge.WithLabels(item.name, "processingTime").Set(item.metrics.processingTime.average);
                            gauge.WithLabels(item.name, "criticalTime").Set(item.metrics.criticalTime.average);
                            gauge.WithLabels(item.name, "retries").Set(item.metrics.retries.average);
                            gauge.WithLabels(item.name, "throughput").Set(item.metrics.throughput.average);
                            gauge.WithLabels(item.name, "queueLength").Set(item.metrics.queueLength.average);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}