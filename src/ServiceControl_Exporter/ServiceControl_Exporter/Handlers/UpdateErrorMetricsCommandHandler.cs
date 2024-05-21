namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Prometheus;

public sealed class UpdateErrorMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration) : IRequestHandler<UpdateErrorMetricsCommand>
{
    public async Task Handle(UpdateErrorMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!configuration.Include.Errors) return;

            var url = Url.Combine(configuration.ServiceControl.Url,
                                  "errors");

            var response = await $"{url}?status=unresolved".SendAsync(HttpMethod.Head, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (response.Headers.TryGetFirst("Total-Count", out var totalCountString) &&
                double.TryParse(totalCountString, out var totalCount))
            {
                if (!metrics.ContainsKey("servicecontrol_errors_unresolved"))
                    metrics.Add("servicecontrol_errors_unresolved", Metrics.CreateGauge("servicecontrol_errors_unresolved", "unresolved error count", "unresolved_error_count"));
                (metrics["servicecontrol_errors_unresolved"] as Gauge)?.Set(totalCount);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}