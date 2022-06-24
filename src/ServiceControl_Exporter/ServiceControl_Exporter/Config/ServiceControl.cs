namespace ServiceControl_Exporter.Config;

public class ServiceControl
{
    public string Url { get; set; } = "http://localhost.com:33333/api";
    public string MonitoringUrl { get; set; } = "http://localhost:33633/api";
}