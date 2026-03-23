using System;
using System.Collections;

namespace XRPerformanceLab.Core.Interfaces
{
    /// <summary>
    /// Manages the lifecycle execution of performance experiments with configurable hold duration.
    /// Executes experiments through: Setup() → Run() → WaitForSeconds(HoldDuration) → Teardown()
    /// </summary>
    public interface IExperimentRunner
    {
        /// <summary>
        /// Invoked when an experiment completes its full lifecycle (Setup → Run → Hold → Teardown).
        /// Passes the experiment ID as the event argument.
        /// </summary>
        event Action<string> OnExperimentCompleted;

        /// <summary>
        /// Gets or sets the duration (in seconds) to hold between Run() and Teardown().
        /// This allows metrics to stabilize before cleanup. Default is 3 seconds.
        /// </summary>
        float HoldDuration { get; set; }

        /// <summary>
        /// Gets whether an experiment is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Executes an experiment coroutine through its full lifecycle: Setup → Run → Hold → Teardown.
        /// </summary>
        /// <param name="experiment">The experiment to execute.</param>
        /// <returns>Coroutine enumerator for use with StartCoroutine().</returns>
        IEnumerator RunExperimentRoutine(IExperiment experiment);
    }
}