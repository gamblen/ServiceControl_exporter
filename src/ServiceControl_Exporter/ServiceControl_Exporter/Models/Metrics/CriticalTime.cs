// ReSharper disable InconsistentNaming
namespace ServiceControl_Exporter.Models.Metrics
{
    public class CriticalTime
    {
        public double average { get; set; }
        public List<double> points { get; set; }
    }
}
