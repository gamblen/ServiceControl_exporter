namespace ServiceControl_Exporter.Models.ServiceControl.Monitoring;

using System.Text.Json.Serialization;

public sealed class Retries
{
    [JsonPropertyName("average")]
    public double Average { get; set; }

    [JsonPropertyName("points")]
    public List<double> Points { get; set; }
}