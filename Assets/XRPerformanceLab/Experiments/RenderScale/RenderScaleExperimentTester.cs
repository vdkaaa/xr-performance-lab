using UnityEngine;
using UnityEngine.Rendering.Universal;
using XRPerformanceLab.Core.Interfaces;
using XRPerformanceLab.Core.Services;

namespace XRPerformanceLab.Experiments.RenderScale
{
    public sealed class RenderScaleExperimentTester : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private UniversalRenderPipelineAsset pipelineAsset;

        [Header("Experiment Settings")]
        [SerializeField] private float targetRenderScale = 1.2f;
        [SerializeField] private bool activateOnStart = true;

        private IExperimentRegistry _registry;
        private IExperimentRunner _runner;

        private const string ExperimentId = "render-scale";

        private void Awake()
        {
            if (pipelineAsset == null)
            {
                Debug.LogError("[RenderScaleExperimentTester] Missing URP pipeline asset reference.");
                enabled = false;
                return;
            }

            _registry = new ExperimentRegistry();
            _runner = new ExperimentRunner(_registry);

            var experiment = new RenderScaleExperiment(
                ExperimentId,
                "Render Scale Experiment",
                pipelineAsset,
                targetRenderScale
            );

            _registry.Register(experiment);

            Debug.Log("[RenderScaleExperimentTester] Render scale experiment registered");
        }

        private void Start()
        {
            if (!activateOnStart)
                return;

            _runner.Activate(ExperimentId);
        }

        private void OnDestroy()
        {
            _runner?.DeactivateAll();
        }
    }
}