namespace ServiceControl_Exporter.Config;

public sealed class Include
{
    public bool CustomChecks { get; set; } = true;
    public bool Errors { get; set; } = true;
    public bool HeartBeats { get; set; } = true;
    public bool Monitoring { get; set; } = true;
    public bool Endpoints { get; set; } = true;
}