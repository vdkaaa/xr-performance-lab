using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments.RenderScale
{
    public sealed class RenderScaleExperiment : IExperiment
    {
        private readonly UniversalRenderPipelineAsset _pipelineAsset;
        private readonly float _targetRenderScale;

        private float _originalRenderScale;
        private bool _hasStoredOriginalValue;

        public string Id { get; }
        public string DisplayName { get; }
        public bool IsActive { get; private set; }

        public RenderScaleExperiment(
            string id,
            string displayName,
            UniversalRenderPipelineAsset pipelineAsset,
            float targetRenderScale)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Experiment id cannot be null or empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty.", nameof(displayName));

            if (pipelineAsset == null)
                throw new ArgumentNullException(nameof(pipelineAsset));

            if (targetRenderScale <= 0f)
                throw new ArgumentOutOfRangeException(nameof(targetRenderScale), "Render scale must be greater than zero.");

            Id = id;
            DisplayName = displayName;
            _pipelineAsset = pipelineAsset;
            _targetRenderScale = targetRenderScale;
        }

        public void Activate()
        {
            if (IsActive)
                return;

            if (!_hasStoredOriginalValue)
            {
                _originalRenderScale = _pipelineAsset.renderScale;
                _hasStoredOriginalValue = true;
            }

            _pipelineAsset.renderScale = _targetRenderScale;
            IsActive = true;

            Debug.Log($"[RenderScaleExperiment] Activated '{DisplayName}' -> Render Scale: {_targetRenderScale:F2}");
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            if (_hasStoredOriginalValue)
                _pipelineAsset.renderScale = _originalRenderScale;

            IsActive = false;

            Debug.Log($"[RenderScaleExperiment] Deactivated '{DisplayName}' -> Restored Render Scale: {_originalRenderScale:F2}");
        }
    }
}