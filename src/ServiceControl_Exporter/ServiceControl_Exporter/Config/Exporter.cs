namespace ServiceControl_Exporter.Config;

public sealed class Exporter
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 9002;
}