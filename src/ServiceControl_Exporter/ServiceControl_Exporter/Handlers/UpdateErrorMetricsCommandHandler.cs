namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Prometheus;

public class UpdateErrorMetricsCommandHandler : IRequestHandler<UpdateErrorMetricsCommand>
{
    private readonly CollectorDictionary _metrics;
    private readonly AppSettings _configuration;

    public UpdateErrorMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration)
    {
        _metrics = metrics;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(UpdateErrorMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_configuration.Include.Errors) return Unit.Value;

            var url = Url.Combine(_configuration.ServiceControl.Url,
                                  "errors");

            var response = await $"{url}?status=unresolved".SendAsync(HttpMethod.Head, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (response.Headers.TryGetFirst("Total-Count", out var totalCountString) &&
                double.TryParse(totalCountString, out var totalCount))
            {
                if (!_metrics.ContainsKey("servicecontrol_errors_unresolved"))
                    _metrics.Add("servicecontrol_errors_unresolved", Metrics.CreateGauge("servicecontrol_errors_unresolved", "unresolved error count", "unresolved_error_count"));
                (_metrics["servicecontrol_errors_unresolved"] as Gauge)?.Set(totalCount);
            }
        }
        catch (Exception)
        {
            // TODO: error handling
        }

        return Unit.Value;
    }
}