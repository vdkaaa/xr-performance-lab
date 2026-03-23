using System;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Core.Services
{
    /// <summary>
    /// Executes performance experiments through their full lifecycle with guaranteed cleanup.
    /// Ensures Teardown() is always called via try/finally, even if Run() throws an exception.
    /// </summary>
    public sealed class ExperimentRunner : IExperimentRunner
    {
        /// <summary>
        /// Invoked when an experiment completes its full lifecycle (Setup → Run → Teardown).
        /// Passes the experiment ID as the event argument.
        /// </summary>
        public event Action<string> OnExperimentCompleted;

        public bool IsRunning { get; private set; }

        /// <summary>
        /// Executes an experiment through its full lifecycle: Setup → Run → Teardown.
        /// If an experiment is already running, logs a warning and returns without executing.
        /// Teardown is guaranteed to execute via try/finally, even if Run() throws an exception.
        /// </summary>
        /// <param name="experiment">The experiment to execute.</param>
        public void RunExperiment(IExperiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            if (IsRunning)
            {
                Debug.LogWarning($"[ExperimentRunner] Cannot run experiment '{experiment.DisplayName}' - another experiment is already running.");
                return;
            }

            IsRunning = true;

            try
            {
                Debug.Log($"[ExperimentRunner] Starting experiment: {experiment.DisplayName} (ID: {experiment.Id})");

                experiment.Setup();
                experiment.Run();
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