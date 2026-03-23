using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using XRPerformanceLab.Core.Interfaces;
using XRPerformanceLab.Core.Services;
using XRPerformanceLab.Experiments.MSAA;
using XRPerformanceLab.Experiments.CPUThrottle;
using XRPerformanceLab.Experiments.RenderScale;
using XRPerformanceLab.Experiments.Shadows;
using XRPerformanceLab.Experiments.Transparency;

namespace XRPerformanceLab.Core
{
    /// <summary>
    /// Central Bootstrap for XR Performance Lab.
    /// This is the ONLY MonoBehaviour allowed to create and wire experiment dependencies.
    /// Registers all experiments and provides access to the ExperimentRunner.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class XRLabBootstrap : MonoBehaviour
    {
        [Header("Pipeline Configuration")]
        [SerializeField] private UniversalRenderPipelineAsset pipelineAsset;

        [Header("MSAA Experiment Configuration")]
        [SerializeField] private int[] msaaLevels = { 0, 2, 4, 8 };

        [Header("CPU Throttle Experiment Configuration")]
        [SerializeField] private int[] cpuTargetRates = { -1, 30, 45, 72, 90 };

        [Header("Render Scale Experiment Configuration")]
        [SerializeField] private float[] renderScaleValues = { 0.5f, 0.75f, 1.0f, 1.5f };

        [Header("Shadow Experiment Configuration")]
        [SerializeField] private float[] shadowDistanceValues = { 0f, 50f, 100f, 150f };

    [Header("Experiment Hold Duration")]
    [SerializeField] private float holdDuration = 3f;

        public IExperimentRunner Runner { get; private set; }
        public List<IExperiment> AllExperiments { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // Create or get ExperimentRunner component on this GameObject
            Runner = GetComponent<ExperimentRunner>();
            if (Runner == null)
            {
                Runner = gameObject.AddComponent<ExperimentRunner>();
            }

            // Configure hold duration
            Runner.HoldDuration = holdDuration;

            AllExperiments = new List<IExperiment>();

            RegisterExperiments();

            Debug.Log($"[XRLabBootstrap] Initialized with {AllExperiments.Count} experiments (Hold Duration: {holdDuration:F1}s).");
        }

        private void RegisterExperiments()
        {
            if (pipelineAsset == null)
            {
                Debug.LogError("[XRLabBootstrap] URP Pipeline Asset not assigned! Some experiments will not be registered.");
            }

            // Register MSAA experiments
            for (int i = 0; i < msaaLevels.Length; i++)
            {
                var level = msaaLevels[i];
                var experiment = new MSAAExperiment(
                    $"msaa-{level}",
                    $"MSAA {level}x",
                    level
                );
                AllExperiments.Add(experiment);
            }

            // Register CPU Throttle experiments
            for (int i = 0; i < cpuTargetRates.Length; i++)
            {
                var rate = cpuTargetRates[i];
                string displayRate = rate == -1 ? "Unlimited" : $"{rate} FPS";
                var experiment = new CPUThrottleExperiment(
                    $"cpu-{rate}",
                    $"CPU Target: {displayRate}",
                    rate
                );
                AllExperiments.Add(experiment);
            }

            // Register Render Scale experiments
            if (pipelineAsset != null)
            {
                for (int i = 0; i < renderScaleValues.Length; i++)
                {
                    var scale = renderScaleValues[i];
                    var experiment = new RenderScaleExperiment(
                        $"render-scale-{scale:F2}",
                        $"Render Scale {scale:F2}x",
                        pipelineAsset,
                        scale
                    );
                    AllExperiments.Add(experiment);
                }
            }

            // Register Shadow Distance experiments
            if (pipelineAsset != null)
            {
                for (int i = 0; i < shadowDistanceValues.Length; i++)
                {
                    var distance = shadowDistanceValues[i];
                    var experiment = new ShadowExperiment(
                        $"shadow-{distance}",
                        $"Shadow Distance {distance}m",
                        pipelineAsset,
                        distance
                    );
                    AllExperiments.Add(experiment);
                }
            }

            // Register Transparency experiment
            var transparencyExperiment = new TransparencyExperiment(
                "transparency-overdraw",
                "Transparency Overdraw Test",
                transparentObjects
            );
            AllExperiments.Add(transparencyExperiment);
        }

        /// <summary>
        /// Finds an experiment by its ID and runs it through the ExperimentRunner as a coroutine.
        /// </summary>
        /// <param name="experimentId">The unique identifier of the experiment to run.</param>
        public void RunExperiment(string experimentId)
        {
            if (string.IsNullOrWhiteSpace(experimentId))
            {
                Debug.LogError("[XRLabBootstrap] Cannot run experiment: experimentId is null or empty.");
                return;
            }

            var experiment = AllExperiments.Find(e => e.Id == experimentId);

            if (experiment == null)
            {
                Debug.LogError($"[XRLabBootstrap] Experiment with ID '{experimentId}' not found.");
                return;
            }

            StartCoroutine(Runner.RunExperimentRoutine(experiment));
        }

        /// <summary>
        /// Gets an experiment by its ID.
        /// </summary>
        /// <param name="experimentId">The unique identifier of the experiment.</param>
        /// <returns>The experiment if found; otherwise null.</returns>
        public IExperiment GetExperimentById(string experimentId)
        {
            return AllExperiments.Find(e => e.Id == experimentId);
        }
    }
}
