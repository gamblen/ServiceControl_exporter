namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Prometheus;

public class UpdateHeartBeatMetricsCommandHandler : IRequestHandler<UpdateHeartBeatMetricsCommand>
{
    private readonly CollectorDictionary _metrics;
    private readonly AppSettings _configuration;

    public UpdateHeartBeatMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration)
    {
        _metrics = metrics;
        _configuration = configuration;
    }

    public async Task Handle(UpdateHeartBeatMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_configuration.Include.HeartBeats) return;

            var url = Url.Combine(_configuration.ServiceControl.Url,
                                  "heartbeats",
                                  "stats");

            var response = await url.GetJsonAsync(cancellationToken).ConfigureAwait(false);

            if (!_metrics.ContainsKey("servicecontrol_heartbeats_stats"))
                _metrics.Add("servicecontrol_heartbeats_stats", Metrics.CreateGauge("servicecontrol_heartbeats_stats", "heartbeats stats", "status"));

            if (_metrics["servicecontrol_heartbeats_stats"] is Gauge gauge)
            {
                gauge.WithLabels("active").Set(response.active);
                gauge.WithLabels("failing").Set(response.failing);
            }
        }
        catch (Exception)
        {
            // TODO: error handling
        }
    }
}