// ReSharper disable InconsistentNaming
namespace ServiceControl_Exporter.Models.Metrics;

public class Retries
{
    public double average { get; set; }
    public List<double> points { get; set; }
}