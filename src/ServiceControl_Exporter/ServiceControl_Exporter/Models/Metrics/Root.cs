// ReSharper disable InconsistentNaming
namespace ServiceControl_Exporter.Models.Metrics;

public class Root
{
    public string name { get; set; }
    public bool isStale { get; set; }
    public List<string> endpointInstanceIds { get; set; }
    public Metrics metrics { get; set; }
    public int disconnectedCount { get; set; }
    public int connectedCount { get; set; }
}