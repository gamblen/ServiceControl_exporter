namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using Mediator;
using Prometheus;

public sealed class UpdateCustomChecksMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration) : ICommandHandler<UpdateCustomChecksMetricsCommand>
{
    public async ValueTask<Unit> Handle(UpdateCustomChecksMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!configuration.Include.CustomChecks) return Unit.Value;

            var url = Url.Combine(configuration.ServiceControl.Url,
                                  "customchecks");

            var response = await url.GetJsonAsync<List<Models.ServiceControl.CustomChecks.Root>>(cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!metrics.ContainsKey("servicecontrol_custom_checks"))
                metrics.Add("servicecontrol_custom_checks", Metrics.CreateGauge("servicecontrol_custom_checks", "custom checks", "category", "custom_check", "status"));

            if (metrics["servicecontrol_custom_checks"] is Gauge gauge)
            {
                foreach (var item in response)
                {
                    gauge.WithLabels(item.Category, item.CustomCheckId, "pass").Set(item.Status == "pass" ? 1 : 0);
                    gauge.WithLabels(item.Category, item.CustomCheckId, "fail").Set(item.Status == "fail" ? 1 : 0);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Unit.Value;
    }
}