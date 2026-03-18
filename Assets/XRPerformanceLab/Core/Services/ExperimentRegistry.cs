using System;
using System.Collections.Generic;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Core.Services
{
    public sealed class ExperimentRegistry : IExperimentRegistry
    {
        private readonly List<IExperiment> _experiments = new();
        private readonly Dictionary<string, IExperiment> _byId = new(StringComparer.Ordinal);

        public IReadOnlyList<IExperiment> GetAll()
        {
            return _experiments;
        }

        public IExperiment GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            _byId.TryGetValue(id, out var experiment);
            return experiment;
        }

        public void Register(IExperiment experiment)
        {
            if (experiment == null)
                throw new ArgumentNullException(nameof(experiment));

            if (string.IsNullOrWhiteSpace(experiment.Id))
                throw new ArgumentException("Experiment Id cannot be null or empty.", nameof(experiment));

            if (_byId.ContainsKey(experiment.Id))
                throw new InvalidOperationException($"An experiment with id '{experiment.Id}' is already registered.");

            _experiments.Add(experiment);
            _byId.Add(experiment.Id, experiment);
        }
    }
}