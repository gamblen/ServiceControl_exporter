namespace ServiceControl_Exporter.Models.ServiceControl.Monitoring;

using System.Text.Json.Serialization;

public sealed class Metrics
{
    [JsonPropertyName("processingTime")]
    public ProcessingTime ProcessingTime { get; set; }

    [JsonPropertyName("criticalTime")]
    public CriticalTime CriticalTime { get; set; }

    [JsonPropertyName("retries")]
    public Retries Retries { get; set; }

    [JsonPropertyName("throughput")]
    public Throughput Throughput { get; set; }

    [JsonPropertyName("queueLength")]
    public QueueLength QueueLength { get; set; }
}