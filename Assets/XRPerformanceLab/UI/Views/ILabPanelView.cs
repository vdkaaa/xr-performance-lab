using XRPerformanceLab.Core.Models;

namespace XRPerformanceLab.UI.Views
{
    public interface ILabPanelView
    {
        void RenderMetrics(PerformanceMetrics metrics);
    }
}