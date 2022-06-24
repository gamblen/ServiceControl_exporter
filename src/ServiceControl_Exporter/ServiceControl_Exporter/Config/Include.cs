namespace ServiceControl_Exporter.Config;

public class Include
{
    public bool CustomChecks { get; set; } = true;
    public bool Errors { get; set; } = true;
    public bool HeartBeats { get; set; } = true;
    public bool Monitoring { get; set; } = true;
}