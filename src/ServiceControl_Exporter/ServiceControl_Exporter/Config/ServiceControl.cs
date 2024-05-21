namespace ServiceControl_Exporter.Config;

public sealed class ServiceControl
{
    public string Url { get; set; }
    public string[] MonitoringUrls { get; set; }
}