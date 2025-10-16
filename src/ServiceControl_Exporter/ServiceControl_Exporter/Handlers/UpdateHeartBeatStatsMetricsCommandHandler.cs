namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using Mediator;
using Prometheus;

public sealed class UpdateHeartBeatStatsMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration) : ICommandHandler<UpdateHeartBeatStatsMetricsCommand>
{
    public async ValueTask<Unit> Handle(UpdateHeartBeatStatsMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!configuration.Include.HeartBeats) return Unit.Value;

            var url = Url.Combine(configuration.ServiceControl.Url,
                                  "heartbeats",
                                  "stats");

            var response = await url.GetJsonAsync<Models.ServiceControl.HeartBeatStats.Root>(cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!metrics.ContainsKey("servicecontrol_heartbeats_stats"))
                metrics.Add("servicecontrol_heartbeats_stats", Metrics.CreateGauge("servicecontrol_heartbeats_stats", "heartbeats stats", "status"));

            if (metrics["servicecontrol_heartbeats_stats"] is Gauge gauge)
            {
                gauge.WithLabels("active").Set(response.Active);
                gauge.WithLabels("failing").Set(response.Failing);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Unit.Value;
    }
}