namespace ServiceControl_Exporter.Models.ServiceControl.Endpoint;

using System.Text.Json.Serialization;

public sealed class Root
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("host_display_name")]
    public string HostDisplayName { get; set; }

    [JsonPropertyName("monitored")]
    public bool Monitored { get; set; }

    [JsonPropertyName("monitor_heartbeat")]
    public bool MonitorHeartbeat { get; set; }

    [JsonPropertyName("heartbeat_information")]
    public HeartbeatInformation HeartbeatInformation { get; set; }

    [JsonPropertyName("is_sending_heartbeats")]
    public bool IsSendingHeartbeats { get; set; }
}