namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Prometheus;

public sealed class UpdateMonitoringMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration) : IRequestHandler<UpdateMonitoringMetricsCommand>
{
    public async Task Handle(UpdateMonitoringMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!configuration.Include.Monitoring ||
                configuration.ServiceControl.MonitoringUrls == null ||
                !configuration.ServiceControl.MonitoringUrls.Any())
                return;

            if (!metrics.ContainsKey("servicecontrol_monitoring_endpoints"))
                //_metrics.Add("servicecontrol_monitoring_endpoints", Metrics.CreateHistogram("servicecontrol_monitoring_endpoints", "monitoring endpoints", "endpoint", "metric"));
                metrics.Add("servicecontrol_monitoring_endpoints", Metrics.CreateGauge("servicecontrol_monitoring_endpoints", "monitoring endpoints", "endpoint", "metric"));

            foreach (var monitoringUrl in configuration.ServiceControl.MonitoringUrls)
            {
                try
                {
                    var url = Url.Combine(monitoringUrl, "monitored-endpoints");


                    var response = await url.SetQueryParam("history", "1").GetJsonAsync<IList<Models.ServiceControl.Monitoring.Root>>(cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (metrics["servicecontrol_monitoring_endpoints"] is Gauge gauge)
                    //if (_metrics["servicecontrol_monitoring_endpoints"] is Histogram histogram)
                    {
                        foreach (var item in response)
                        {
                            //(histogram.WithLabels(item.name, "processingTime") as Histogram)?.Observe(item.metrics.processingTime.average);
                            //(histogram.WithLabels(item.name, "criticalTime") as Histogram)?.Observe(item.metrics.criticalTime.average);
                            //(histogram.WithLabels(item.name, "retries") as Histogram)?.Observe(item.metrics.retries.average);
                            //(histogram.WithLabels(item.name, "throughput") as Histogram)?.Observe(item.metrics.throughput.average);
                            //(histogram.WithLabels(item.name, "queueLength") as Histogram)?.Observe(item.metrics.queueLength.average);

                            gauge.WithLabels(item.Name, "processingTime").Set(item.Metrics.ProcessingTime.Average);
                            gauge.WithLabels(item.Name, "criticalTime").Set(item.Metrics.CriticalTime.Average);
                            gauge.WithLabels(item.Name, "retries").Set(item.Metrics.Retries.Average);
                            gauge.WithLabels(item.Name, "throughput").Set(item.Metrics.Throughput.Average);
                            gauge.WithLabels(item.Name, "queueLength").Set(item.Metrics.QueueLength.Average);
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