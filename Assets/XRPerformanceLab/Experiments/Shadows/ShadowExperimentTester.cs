using UnityEngine;
using UnityEngine.Rendering.Universal;
using XRPerformanceLab.Core.Interfaces;
using XRPerformanceLab.Core.Services;

namespace XRPerformanceLab.Experiments.Shadows
{
    public sealed class ShadowExperimentTester : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private UniversalRenderPipelineAsset pipelineAsset;

        [Header("Experiment Settings")]
        [SerializeField] private float targetShadowDistance = 5f;
        [SerializeField] private bool activateOnStart = true;

        private IExperimentRegistry _registry;
        private IExperimentRunner _runner;

        private const string ExperimentId = "shadow-experiment";

        private void Awake()
        {
            if (pipelineAsset == null)
            {
                Debug.LogError("[ShadowExperimentTester] Missing URP pipeline asset reference.");
                enabled = false;
                return;
            }

            _registry = new ExperimentRegistry();
            _runner = new ExperimentRunner(_registry);

            var experiment = new ShadowExperiment(
                ExperimentId,
                "Shadow Experiment",
                pipelineAsset,
                targetShadowDistance
            );

            _registry.Register(experiment);

            Debug.Log("[ShadowExperimentTester] Shadow experiment registered");
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