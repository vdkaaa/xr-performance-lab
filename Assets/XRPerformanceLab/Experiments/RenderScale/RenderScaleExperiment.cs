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

        public string Id { get; }
        public string DisplayName { get; }

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

        public void Setup()
        {
            _originalRenderScale = _pipelineAsset.renderScale;
            Debug.Log($"[RenderScaleExperiment] Setup '{DisplayName}' -> Saved original render scale: {_originalRenderScale:F2}");
        }

        public void Run()
        {
            _pipelineAsset.renderScale = _targetRenderScale;
            Debug.Log($"[RenderScaleExperiment] Run '{DisplayName}' -> Render Scale: {_targetRenderScale:F2}");
        }

        public void Teardown()
        {
            _pipelineAsset.renderScale = _originalRenderScale;
            Debug.Log($"[RenderScaleExperiment] Teardown '{DisplayName}' -> Restored Render Scale: {_originalRenderScale:F2}");
        }
    }
}