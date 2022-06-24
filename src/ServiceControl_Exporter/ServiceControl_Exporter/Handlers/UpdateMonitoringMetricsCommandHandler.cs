namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Prometheus;

public class UpdateMonitoringMetricsCommandHandler : IRequestHandler<UpdateMonitoringMetricsCommand>
{
    private readonly CollectorDictionary _metrics;
    private readonly AppSettings _configuration;

    public UpdateMonitoringMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration)
    {
        _metrics = metrics;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(UpdateMonitoringMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_configuration.Include.Monitoring) return Unit.Value;

            var url = Url.Combine(_configuration.ServiceControl.MonitoringUrl,
                                  "monitored-endpoints");

            var response = await url.SetQueryParam("history", "1").GetJsonListAsync(cancellationToken).ConfigureAwait(false);

            if (!_metrics.ContainsKey("servicecontrol_monitoring_endpoints"))
                _metrics.Add("servicecontrol_monitoring_endpoints", Metrics.CreateHistogram("servicecontrol_monitoring_endpoints", "monitoring endpoints", "endpoint", "metric"));

            if (_metrics["servicecontrol_heartbeats_stats"] is Gauge gauge)
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
        catch (Exception)
        {
            // TODO: error handling
        }

        return Unit.Value;
    }
}