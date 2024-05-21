namespace ServiceControl_Exporter.Models.ServiceControl.Monitoring;

using System.Text.Json.Serialization;

public sealed class Throughput
{
    [JsonPropertyName("average")]
    public double Average { get; set; }

    [JsonPropertyName("points")]
    public List<double> Points { get; set; }
}