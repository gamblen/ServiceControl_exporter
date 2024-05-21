namespace ServiceControl_Exporter.Models.ServiceControl.HeartBeatStats;

using System.Text.Json.Serialization;

public sealed class Root
{
    [JsonPropertyName("active")]
    public int Active { get; set; }

    [JsonPropertyName("failing")]
    public int Failing { get; set; }
}