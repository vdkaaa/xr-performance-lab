using TMPro;
using UnityEngine;
using XRPerformanceLab.Core.Models;
using XRPerformanceLab.UI.Views;

namespace XRPerformanceLab.UI.Views
{
    public sealed class BasicLabMetricsView : MonoBehaviour, ILabPanelView
    {
        [Header("Metric Labels")]
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private TMP_Text frameTimeText;
        [SerializeField] private TMP_Text batchesText;
        [SerializeField] private TMP_Text trianglesText;
        [SerializeField] private TMP_Text verticesText;

        public void RenderMetrics(PerformanceMetrics metrics)
        {
            if (fpsText != null)
                fpsText.text = $"FPS: {metrics.Fps:F1}";

            if (frameTimeText != null)
                frameTimeText.text = $"Frame: {metrics.FrameTimeMs:F2} ms";

            if (batchesText != null)
                batchesText.text = $"Batches: {metrics.Batches}";

            if (trianglesText != null)
                trianglesText.text = $"Triangles: {metrics.Triangles}";

            if (verticesText != null)
                verticesText.text = $"Vertices: {metrics.Vertices}";
        }
    }
}