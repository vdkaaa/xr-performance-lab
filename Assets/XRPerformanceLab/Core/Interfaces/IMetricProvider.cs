using XRPerformanceLab.Core.Models;

namespace XRPerformanceLab.Metrics.Interfaces
{
    public interface IMetricProvider
    {
        PerformanceMetrics GetCurrent();
    }
}