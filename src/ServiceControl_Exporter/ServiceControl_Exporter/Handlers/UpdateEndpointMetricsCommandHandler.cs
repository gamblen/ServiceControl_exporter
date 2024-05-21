namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Prometheus;

public sealed class UpdateEndpointMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration) : IRequestHandler<UpdateEndpointStatsMetricsCommand>
{
    public async Task Handle(UpdateEndpointStatsMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!configuration.Include.Endpoints) return;

            var url = Url.Combine(configuration.ServiceControl.Url,
                                  "endpoints");

            var response = await url.GetJsonAsync<List<Models.ServiceControl.Endpoint.Root>>(cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!metrics.ContainsKey("servicecontrol_endpoints"))
                metrics.Add("servicecontrol_endpoints", Metrics.CreateGauge("servicecontrol_endpoints", "endpoint stats", "name", "host", "heartbeat_status"));

            if (metrics["servicecontrol_endpoints"] is Gauge gauge)
            {
                foreach (var item in response.Where(x => x.Monitored))
                {
                    gauge.WithLabels(item.Name, item.HostDisplayName, "beating").Set(item.HeartbeatInformation.ReportedStatus == "beating" ? 1 : 0);
                    gauge.WithLabels(item.Name, item.HostDisplayName, "dead").Set(item.HeartbeatInformation.ReportedStatus == "dead" ? 1 : 0);
                    gauge.WithLabels(item.Name, item.HostDisplayName, "unknown").Set(item.HeartbeatInformation.ReportedStatus == "unknown" ? 1 : 0);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}