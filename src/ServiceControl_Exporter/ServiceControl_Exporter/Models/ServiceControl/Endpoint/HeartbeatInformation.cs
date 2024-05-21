namespace ServiceControl_Exporter.Models.ServiceControl.Endpoint;

using System.Text.Json.Serialization;

public sealed class HeartbeatInformation
{
    [JsonPropertyName("last_report_at")]
    public DateTime LastReportAt { get; set; }

    [JsonPropertyName("reported_status")]
    public string ReportedStatus { get; set; }
}