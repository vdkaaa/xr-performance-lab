using System;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments
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
        private bool _hasStoredOriginalValue;

        public string Id { get; }
        public string DisplayName { get; }
        public bool IsActive { get; private set; }

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

        public void Activate()
        {
            if (IsActive)
                return;

            if (!_hasStoredOriginalValue)
            {
                _originalFrameRate = Application.targetFrameRate;
                _hasStoredOriginalValue = true;
            }

            Application.targetFrameRate = _targetFrameRate;
            IsActive = true;

            string rateDisplay = _targetFrameRate == -1 ? "Unlimited" : $"{_targetFrameRate} FPS";
            Debug.Log($"[CPUThrottleExperiment] Activated '{DisplayName}' -> Target Frame Rate: {rateDisplay}");
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            if (_hasStoredOriginalValue)
                Application.targetFrameRate = _originalFrameRate;

            IsActive = false;

            string rateDisplay = _originalFrameRate == -1 ? "Unlimited" : $"{_originalFrameRate} FPS";
            Debug.Log($"[CPUThrottleExperiment] Deactivated '{DisplayName}' -> Restored Target Frame Rate: {rateDisplay}");
        }
    }
}
