using UnityEngine;
using UnityEngine.Rendering.Universal;
using XRPerformanceLab.Core.Interfaces;
using XRPerformanceLab.Core.Services;
using XRPerformanceLab.Experiments.RenderScale;
using XRPerformanceLab.Experiments.Shadows;
using XRPerformanceLab.UI.Views;

namespace XRPerformanceLab.UI.Panels
{
    public sealed class ExperimentControlPanelController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private UniversalRenderPipelineAsset pipelineAsset;
        [SerializeField] private BasicExperimentControlView controlView;

        [Header("Experiment Settings")]
        [SerializeField] private float renderScaleTarget = 1.2f;
        [SerializeField] private float shadowDistanceOff = 0f;

        private IExperimentRegistry _registry;
        private IExperimentRunner _runner;

        private const string RenderScaleExperimentId = "render-scale";
        private const string ShadowExperimentId = "shadow-distance";

        private void Awake()
        {
            if (pipelineAsset == null)
            {
                Debug.LogError("[ExperimentControlPanelController] Missing URP pipeline asset reference.");
                enabled = false;
                return;
            }

            if (controlView == null)
            {
                Debug.LogError("[ExperimentControlPanelController] Missing control view reference.");
                enabled = false;
                return;
            }

            _registry = new ExperimentRegistry();
            _runner = new ExperimentRunner(_registry);

            RegisterExperiments();
            BindButtons();

            controlView.SetStatus("Experiments ready.");
        }

        private void RegisterExperiments()
        {
            var renderScaleExperiment = new RenderScaleExperiment(
                RenderScaleExperimentId,
                "Render Scale Experiment",
                pipelineAsset,
                renderScaleTarget
            );

            var shadowExperiment = new ShadowExperiment(
                ShadowExperimentId,
                "Shadow Distance Experiment",
                pipelineAsset,
                shadowDistanceOff
            );

            _registry.Register(renderScaleExperiment);
            _registry.Register(shadowExperiment);
        }

        private void BindButtons()
        {
            controlView.RenderScaleOnButton.onClick.AddListener(OnRenderScaleActivated);
            controlView.RenderScaleOffButton.onClick.AddListener(OnRenderScaleDeactivated);
            controlView.ShadowsOffButton.onClick.AddListener(OnShadowsDisabled);
            controlView.ShadowsRestoreButton.onClick.AddListener(OnShadowsRestored);
        }

        private void OnRenderScaleActivated()
        {
            _runner.Activate(RenderScaleExperimentId);
            controlView.SetStatus($"Activated: {RenderScaleExperimentId}");
        }

        private void OnRenderScaleDeactivated()
        {
            _runner.Deactivate(RenderScaleExperimentId);
            controlView.SetStatus($"Deactivated: {RenderScaleExperimentId}");
        }

        private void OnShadowsDisabled()
        {
            _runner.Activate(ShadowExperimentId);
            controlView.SetStatus($"Activated: {ShadowExperimentId}");
        }

        private void OnShadowsRestored()
        {
            _runner.Deactivate(ShadowExperimentId);
            controlView.SetStatus($"Deactivated: {ShadowExperimentId}");
        }

        private void OnDestroy()
        {
            if (controlView != null)
            {
                controlView.RenderScaleOnButton.onClick.RemoveListener(OnRenderScaleActivated);
                controlView.RenderScaleOffButton.onClick.RemoveListener(OnRenderScaleDeactivated);
                controlView.ShadowsOffButton.onClick.RemoveListener(OnShadowsDisabled);
                controlView.ShadowsRestoreButton.onClick.RemoveListener(OnShadowsRestored);
            }

            _runner?.DeactivateAll();
        }
    }
}