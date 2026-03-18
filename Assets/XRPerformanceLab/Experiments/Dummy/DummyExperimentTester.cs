using UnityEngine;
using XRPerformanceLab.Core.Interfaces;
using XRPerformanceLab.Core.Services;

namespace XRPerformanceLab.Experiments.Dummy
{
    public sealed class DummyExperimentTester : MonoBehaviour
    {
        private IExperimentRegistry _registry;
        private IExperimentRunner _runner;

        private void Awake()
        {
            _registry = new ExperimentRegistry();
            _runner = new ExperimentRunner(_registry);

            var dummyExperiment = new DummyExperiment();
            _registry.Register(dummyExperiment);

            Debug.Log("[DummyExperimentTester] Dummy experiment registered");
        }

        private void Start()
        {
            _runner.Activate("dummy");
            _runner.Deactivate("dummy");
        }
    }
}