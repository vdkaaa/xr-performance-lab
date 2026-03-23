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

        public string Id { get; }
        public string DisplayName { get; }

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

        public void Setup()
        {
            _originalShadowDistance = _pipelineAsset.shadowDistance;
            Debug.Log($"[ShadowExperiment] Setup '{DisplayName}' -> Saved original shadow distance: {_originalShadowDistance}");
        }

        public void Run()
        {
            _pipelineAsset.shadowDistance = _targetShadowDistance;
            Debug.Log($"[ShadowExperiment] Run '{DisplayName}' -> Shadow Distance: {_targetShadowDistance}");
        }

        public void Teardown()
        {
            _pipelineAsset.shadowDistance = _originalShadowDistance;
            Debug.Log($"[ShadowExperiment] Teardown '{DisplayName}' -> Restored Shadow Distance: {_originalShadowDistance}");
        }
    }
}