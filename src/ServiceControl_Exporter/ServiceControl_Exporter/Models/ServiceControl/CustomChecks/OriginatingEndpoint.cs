namespace ServiceControl_Exporter.Models.ServiceControl.CustomChecks;

using System.Text.Json.Serialization;

public sealed class OriginatingEndpoint
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("host_id")]
    public string HostId { get; set; }

    [JsonPropertyName("host")]
    public string Host { get; set; }
}