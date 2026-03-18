using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments.Dummy
{
    public sealed class DummyExperiment : IExperiment
    {
        public string Id => "dummy";
        public string DisplayName => "Dummy Experiment";
        public bool IsActive { get; private set; }

        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            Debug.Log("[DummyExperiment] Activated");
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
            Debug.Log("[DummyExperiment] Deactivated");
        }
    }
}