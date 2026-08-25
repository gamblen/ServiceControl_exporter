namespace ServiceControl_Exporter.Config;

public sealed class Exporter
{
    public string Host { get; set; } = "+";
    public ushort Port { get; set; } = 33334;
}