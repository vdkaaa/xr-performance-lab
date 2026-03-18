using UnityEngine;
using XRPerformanceLab.Metrics.Interfaces;

namespace XRPerformanceLab.Metrics.Providers
{
    public sealed class MetricProviderTester : MonoBehaviour
    {
        private IMetricProvider _metricProvider;
        private UnityStatsMetricProvider _unityStatsMetricProvider;
        private float _logTimer;

        [SerializeField] private float logIntervalSeconds = 1f;

        private void Awake()
        {
            _unityStatsMetricProvider = new UnityStatsMetricProvider();
            _metricProvider = _unityStatsMetricProvider;
        }

        private void Update()
        {
            _unityStatsMetricProvider.Tick(Time.unscaledDeltaTime);

            _logTimer += Time.unscaledDeltaTime;

            if (_logTimer < logIntervalSeconds)
                return;

            _logTimer = 0f;

            var metrics = _metricProvider.GetCurrent();

            Debug.Log(
                $"[Metrics] FPS: {metrics.Fps:F1} | " +
                $"Frame: {metrics.FrameTimeMs:F2} ms | " +
                $"Batches: {metrics.Batches} | " +
                $"Tris: {metrics.Triangles} | " +
                $"Verts: {metrics.Vertices}"
            );
        }
    }
}