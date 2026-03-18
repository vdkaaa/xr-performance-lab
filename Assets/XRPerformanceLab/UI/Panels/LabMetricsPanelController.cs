using UnityEngine;
using XRPerformanceLab.Metrics.Interfaces;
using XRPerformanceLab.Metrics.Providers;
using XRPerformanceLab.UI.Views;

namespace XRPerformanceLab.UI.Panels
{
    public sealed class LabMetricsPanelController : MonoBehaviour
    {
        [SerializeField] private BasicLabMetricsView metricsView;
        [SerializeField] private float refreshIntervalSeconds = 0.2f;

        private IMetricProvider _metricProvider;
        private UnityStatsMetricProvider _unityStatsMetricProvider;
        private float _refreshTimer;

        private void Awake()
        {
            _unityStatsMetricProvider = new UnityStatsMetricProvider();
            _metricProvider = _unityStatsMetricProvider;
        }

        private void Update()
        {
            _unityStatsMetricProvider.Tick(Time.unscaledDeltaTime);

            _refreshTimer += Time.unscaledDeltaTime;

            if (_refreshTimer < refreshIntervalSeconds)
                return;

            _refreshTimer = 0f;

            if (metricsView == null)
                return;

            var metrics = _metricProvider.GetCurrent();
            metricsView.RenderMetrics(metrics);
        }
    }
}