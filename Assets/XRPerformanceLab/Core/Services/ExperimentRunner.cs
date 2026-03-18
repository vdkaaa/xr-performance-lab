using System;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Core.Services
{
    public sealed class ExperimentRunner : IExperimentRunner
    {
        private readonly IExperimentRegistry _registry;

        public ExperimentRunner(IExperimentRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public void Activate(string experimentId)
        {
            var experiment = _registry.GetById(experimentId);

            if (experiment == null)
                throw new InvalidOperationException($"Experiment with id '{experimentId}' was not found.");

            if (experiment.IsActive)
                return;

            experiment.Activate();
        }

        public void Deactivate(string experimentId)
        {
            var experiment = _registry.GetById(experimentId);

            if (experiment == null)
                throw new InvalidOperationException($"Experiment with id '{experimentId}' was not found.");

            if (!experiment.IsActive)
                return;

            experiment.Deactivate();
        }

        public void DeactivateAll()
        {
            var experiments = _registry.GetAll();

            for (int i = 0; i < experiments.Count; i++)
            {
                var experiment = experiments[i];

                if (!experiment.IsActive)
                    continue;

                experiment.Deactivate();
            }
        }
    }
}