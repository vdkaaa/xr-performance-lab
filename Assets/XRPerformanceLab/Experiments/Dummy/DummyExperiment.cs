using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments.Dummy
{
    public sealed class DummyExperiment : IExperiment
    {
        public string Id => "dummy";
        public string DisplayName => "Dummy Experiment";

        public void Setup()
        {
            Debug.Log("[DummyExperiment] Setup");
        }

        public void Run()
        {
            Debug.Log("[DummyExperiment] Run");
        }

        public void Teardown()
        {
            Debug.Log("[DummyExperiment] Teardown");
        }
    }
}