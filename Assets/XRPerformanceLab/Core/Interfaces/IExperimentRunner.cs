namespace XRPerformanceLab.Core.Interfaces
{
    /// <summary>
    /// Manages the lifecycle execution of performance experiments.
    /// </summary>
    public interface IExperimentRunner
    {
        /// <summary>
        /// Gets whether an experiment is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Executes an experiment through its full lifecycle: Setup → Run → Teardown.
        /// </summary>
        /// <param name="experiment">The experiment to execute.</param>
        void RunExperiment(IExperiment experiment);
    }
}