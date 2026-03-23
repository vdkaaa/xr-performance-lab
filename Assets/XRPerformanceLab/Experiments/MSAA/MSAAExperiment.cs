using System;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments.MSAA
{
    /// <summary>
    /// Tests the performance impact of Multi-Sample Anti-Aliasing (MSAA) quality levels.
    /// Modifies QualitySettings.antiAliasing to test different MSAA sample counts (0, 2, 4, 8).
    /// Higher MSAA values improve edge quality but increase GPU fill rate cost.
    /// </summary>
    public sealed class MSAAExperiment : IExperiment
    {
        private readonly int _targetMSAALevel;

        private int _originalMSAALevel;
        private bool _hasStoredOriginalValue;

        public string Id { get; }
        public string DisplayName { get; }
        public bool IsActive { get; private set; }

        public MSAAExperiment(
            string id,
            string displayName,
            int targetMSAALevel)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Experiment id cannot be null or empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty.", nameof(displayName));

            if (targetMSAALevel != 0 && targetMSAALevel != 2 && targetMSAALevel != 4 && targetMSAALevel != 8)
                throw new ArgumentOutOfRangeException(nameof(targetMSAALevel), "MSAA level must be 0, 2, 4, or 8.");

            Id = id;
            DisplayName = displayName;
            _targetMSAALevel = targetMSAALevel;
        }

        public void Activate()
        {
            if (IsActive)
                return;

            if (!_hasStoredOriginalValue)
            {
                _originalMSAALevel = QualitySettings.antiAliasing;
                _hasStoredOriginalValue = true;
            }

            QualitySettings.antiAliasing = _targetMSAALevel;
            IsActive = true;

            Debug.Log($"[MSAAExperiment] Activated '{DisplayName}' -> MSAA Level: {_targetMSAALevel}x");
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            if (_hasStoredOriginalValue)
                QualitySettings.antiAliasing = _originalMSAALevel;

            IsActive = false;

            Debug.Log($"[MSAAExperiment] Deactivated '{DisplayName}' -> Restored MSAA Level: {_originalMSAALevel}x");
        }
    }
}
