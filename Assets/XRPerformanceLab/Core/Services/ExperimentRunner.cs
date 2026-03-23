using System;
using System.Collections;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Core.Services
{
    /// <summary>
    /// Executes performance experiments through their full lifecycle with configurable hold duration.
    /// Sequence: Setup() → Run() → Hold (WaitForSeconds) → Teardown()
    /// Teardown() is guaranteed via try/finally, even if Run() throws an exception.
    /// Uses coroutines to allow metric stabilization time between Run() and Teardown().
    /// </summary>
    public sealed class ExperimentRunner : MonoBehaviour, IExperimentRunner
    {
        /// <summary>
        /// Invoked when an experiment completes its full lifecycle (Setup → Run → Hold → Teardown).
        /// Passes the experiment ID as the event argument.
        /// </summary>
        public event Action<string> OnExperimentCompleted;

        /// <summary>
        /// Duration (in seconds) to hold between Run() and Teardown() for metric stabilization.
        /// Default is 3 seconds.
        /// </summary>
        public float HoldDuration { get; set; } = 3f;

        public bool IsRunning { get; private set; }

        /// <summary>
        /// Executes an experiment coroutine through its full lifecycle: Setup → Run → Hold → Teardown.
        /// If an experiment is already running, logs a warning and returns an empty enumerator.
        /// Teardown is guaranteed to execute via try/finally, even if Run() throws an exception.
        /// </summary>
        /// <param name="experiment">The experiment to execute.</param>
        /// <returns>Coroutine enumerator for use with StartCoroutine().</returns>
        public IEnumerator RunExperimentRoutine(IExperiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            if (IsRunning)
            {
                Debug.LogWarning($"[ExperimentRunner] Cannot run experiment '{experiment.DisplayName}' - another experiment is already running.");
                yield break;
            }

            IsRunning = true;
            float holdTimeRemaining = HoldDuration;

            try
            {
                Debug.Log($"[ExperimentRunner] Starting experiment: {experiment.DisplayName} (ID: {experiment.Id})");

                experiment.Setup();
                experiment.Run();

                Debug.Log($"[ExperimentRunner] Holding for {HoldDuration:F1} seconds to stabilize metrics...");

                // Hold for the specified duration, allowing metrics to stabilize
                while (holdTimeRemaining > 0f)
                {
                    yield return null;
                    holdTimeRemaining -= Time.deltaTime;
                }

                Debug.Log($"[ExperimentRunner] Hold duration complete, proceeding to teardown.");
            }
            finally
            {
                experiment.Teardown();
                IsRunning = false;

                Debug.Log($"[ExperimentRunner] Completed experiment: {experiment.DisplayName} (ID: {experiment.Id})");

                OnExperimentCompleted?.Invoke(experiment.Id);
            }
        }
    }
}