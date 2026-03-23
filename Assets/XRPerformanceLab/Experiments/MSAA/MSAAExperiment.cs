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

        public string Id { get; }
        public string DisplayName { get; }

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

        public void Setup()
        {
            _originalMSAALevel = QualitySettings.antiAliasing;
            Debug.Log($"[MSAAExperiment] Setup '{DisplayName}' -> Saved original MSAA level: {_originalMSAALevel}x");
        }

        public void Run()
        {
            QualitySettings.antiAliasing = _targetMSAALevel;
            Debug.Log($"[MSAAExperiment] Run '{DisplayName}' -> MSAA Level: {_targetMSAALevel}x");
        }

        public void Teardown()
        {
            QualitySettings.antiAliasing = _originalMSAALevel;
            Debug.Log($"[MSAAExperiment] Teardown '{DisplayName}' -> Restored MSAA Level: {_originalMSAALevel}x");
        }
    }
}
