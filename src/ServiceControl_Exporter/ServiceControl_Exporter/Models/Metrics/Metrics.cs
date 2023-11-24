// ReSharper disable InconsistentNaming
namespace ServiceControl_Exporter.Models.Metrics;

public class Metrics
{
    public ProcessingTime processingTime { get; set; }
    public CriticalTime criticalTime { get; set; }
    public Retries retries { get; set; }
    public Throughput throughput { get; set; }
    public QueueLength queueLength { get; set; }
}