using UnityEditor;
using UnityEngine;
using XRPerformanceLab.Core.Models;
using XRPerformanceLab.Metrics.Interfaces;

namespace XRPerformanceLab.Metrics.Providers
{
    public sealed class UnityStatsMetricProvider : IMetricProvider
    {
        private float _smoothedDeltaTime;

        public UnityStatsMetricProvider()
        {
            _smoothedDeltaTime = 1f / 60f;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
                return;

            _smoothedDeltaTime = Mathf.Lerp(_smoothedDeltaTime, deltaTime, 0.1f);
        }

        public PerformanceMetrics GetCurrent()
        {
            float frameTimeMs = _smoothedDeltaTime * 1000f;
            float fps = _smoothedDeltaTime > 0f ? 1f / _smoothedDeltaTime : 0f;

            return new PerformanceMetrics
            {
                Fps = fps,
                FrameTimeMs = frameTimeMs,
                DrawCalls = 0,
                Batches = UnityStats.batches,
                Triangles = UnityStats.triangles,
                Vertices = UnityStats.vertices
            };
        }
    }
}