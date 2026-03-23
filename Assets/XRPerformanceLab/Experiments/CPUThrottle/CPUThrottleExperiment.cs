using System;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments.CPUThrottle
{
    /// <summary>
    /// Tests the performance impact of CPU budget constraints via target frame rate limiting.
    /// Modifies Application.targetFrameRate to simulate different CPU performance profiles.
    /// Common values: -1 (unlimited/VSync), 30, 45, 72, 90, 120.
    /// Useful for testing how the application behaves under CPU-constrained scenarios.
    /// </summary>
    public sealed class CPUThrottleExperiment : IExperiment
    {
        private readonly int _targetFrameRate;
        private int _originalFrameRate;

        public string Id { get; }
        public string DisplayName { get; }

        public CPUThrottleExperiment(
            string id,
            string displayName,
            int targetFrameRate)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Experiment id cannot be null or empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty.", nameof(displayName));

            if (targetFrameRate < -1)
                throw new ArgumentOutOfRangeException(nameof(targetFrameRate), "Target frame rate must be -1 (unlimited) or a positive value.");

            Id = id;
            DisplayName = displayName;
            _targetFrameRate = targetFrameRate;
        }

        public void Setup()
        {
            _originalFrameRate = Application.targetFrameRate;
            Debug.Log($"[CPUThrottleExperiment] Setup '{DisplayName}' -> Saved original frame rate: {(_originalFrameRate == -1 ? "Unlimited" : $"{_originalFrameRate} FPS")}");
        }

        public void Run()
        {
            Application.targetFrameRate = _targetFrameRate;
            string rateDisplay = _targetFrameRate == -1 ? "Unlimited" : $"{_targetFrameRate} FPS";
            Debug.Log($"[CPUThrottleExperiment] Run '{DisplayName}' -> Target Frame Rate: {rateDisplay}");
        }

        public void Teardown()
        {
            Application.targetFrameRate = _originalFrameRate;
            string rateDisplay = _originalFrameRate == -1 ? "Unlimited" : $"{_originalFrameRate} FPS";
            Debug.Log($"[CPUThrottleExperiment] Teardown '{DisplayName}' -> Restored Target Frame Rate: {rateDisplay}");
        }
    }
}
