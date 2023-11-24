namespace ServiceControl_Exporter.Handlers;

using Commands;
using Config;
using Flurl;
using Flurl.Http;
using MediatR;
using Prometheus;

public class UpdateCustomChecksMetricsCommandHandler : IRequestHandler<UpdateCustomChecksMetricsCommand>
{
    private readonly CollectorDictionary _metrics;
    private readonly AppSettings _configuration;

    public UpdateCustomChecksMetricsCommandHandler(CollectorDictionary metrics, AppSettings configuration)
    {
        _metrics = metrics;
        _configuration = configuration;
    }

    public async Task Handle(UpdateCustomChecksMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_configuration.Include.CustomChecks) return;

            var url = Url.Combine(_configuration.ServiceControl.Url,
                                  "customchecks");

            var response = await url.GetJsonListAsync(cancellationToken).ConfigureAwait(false);

            if (!_metrics.ContainsKey("servicecontrol_custom_checks"))
                _metrics.Add("servicecontrol_custom_checks", Metrics.CreateGauge("servicecontrol_custom_checks", "custom checks", "category", "custom_check", "status"));

            if (_metrics["servicecontrol_custom_checks"] is Gauge gauge)
            {
                foreach (var item in response)
                {
                    gauge.WithLabels(item.category, item.custom_check_id, "pass").Set(item.status == "pass" ? 1 : 0);
                    gauge.WithLabels(item.category, item.custom_check_id, "fail").Set(item.status == "fail" ? 1 : 0);
                }
            }
        }
        catch (Exception)
        {
            // TODO: error handling
        }
    }
}