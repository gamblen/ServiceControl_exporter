namespace ServiceControl_Exporter.Config;

public class AppSettings
{
    public Exporter Exporter { get; set; } = new();

    public ServiceControl ServiceControl { get; set; } = new();

    public Include Include { get; set; } = new();

    public string ProxyUrl { get; set; }
}