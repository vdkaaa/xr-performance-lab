using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments.Shadows
{
    public sealed class ShadowExperiment : IExperiment
    {
        private readonly UniversalRenderPipelineAsset _pipelineAsset;
        private readonly float _targetShadowDistance;

        private float _originalShadowDistance;
        private bool _hasStoredOriginalValue;

        public string Id { get; }
        public string DisplayName { get; }
        public bool IsActive { get; private set; }

        public ShadowExperiment(
            string id,
            string displayName,
            UniversalRenderPipelineAsset pipelineAsset,
            float targetShadowDistance)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid id", nameof(id));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Invalid display name", nameof(displayName));

            _pipelineAsset = pipelineAsset ?? throw new ArgumentNullException(nameof(pipelineAsset));

            if (targetShadowDistance < 0f)
                throw new ArgumentOutOfRangeException(nameof(targetShadowDistance));

            Id = id;
            DisplayName = displayName;
            _targetShadowDistance = targetShadowDistance;
        }

        public void Activate()
        {
            if (IsActive)
                return;

            if (!_hasStoredOriginalValue)
            {
                _originalShadowDistance = _pipelineAsset.shadowDistance;
                _hasStoredOriginalValue = true;
            }

            _pipelineAsset.shadowDistance = _targetShadowDistance;
            IsActive = true;

            Debug.Log($"[ShadowExperiment] Activated '{DisplayName}' -> ShadowDistance: {_targetShadowDistance}");
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            if (_hasStoredOriginalValue)
                _pipelineAsset.shadowDistance = _originalShadowDistance;

            IsActive = false;

            Debug.Log($"[ShadowExperiment] Deactivated '{DisplayName}' -> Restored: {_originalShadowDistance}");
        }
    }
}