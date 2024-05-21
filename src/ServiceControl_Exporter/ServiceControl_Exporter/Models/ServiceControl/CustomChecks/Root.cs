namespace ServiceControl_Exporter.Models.ServiceControl.CustomChecks;

using System.Text.Json.Serialization;

public sealed class Root
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("custom_check_id")]
    public string CustomCheckId { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("reported_at")]
    public DateTime ReportedAt { get; set; }

    [JsonPropertyName("originating_endpoint")]
    public OriginatingEndpoint OriginatingEndpoint { get; set; }

    [JsonPropertyName("failure_reason")]
    public string FailureReason { get; set; }
}