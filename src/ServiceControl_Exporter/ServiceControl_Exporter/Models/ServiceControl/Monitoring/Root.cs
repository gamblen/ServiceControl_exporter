namespace ServiceControl_Exporter.Models.ServiceControl.Monitoring;

using System.Text.Json.Serialization;

public sealed class Root
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("isStale")]
    public bool IsStale { get; set; }

    [JsonPropertyName("endpointInstanceIds")]
    public List<string> EndpointInstanceIds { get; set; }

    [JsonPropertyName("metrics")]
    public Metrics Metrics { get; set; }

    [JsonPropertyName("disconnectedCount")]
    public int DisconnectedCount { get; set; }

    [JsonPropertyName("connectedCount")]
    public int ConnectedCount { get; set; }
}