using System;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments
{
    /// <summary>
    /// Tests the performance impact of transparent object overdraw and GPU fill rate.
    /// Toggles a set of transparent GameObjects to simulate varying levels of overdraw stress.
    /// Overdraw occurs when the same pixel is drawn multiple times due to transparent/alpha-blended objects.
    /// Higher overdraw significantly increases GPU fragment shader workload and fill rate pressure.
    /// </summary>
    public sealed class TransparencyExperiment : IExperiment
    {
        private readonly GameObject[] _transparentObjects;

        private bool[] _originalActiveStates;
        private bool _hasStoredOriginalStates;

        public string Id { get; }
        public string DisplayName { get; }
        public bool IsActive { get; private set; }

        public TransparencyExperiment(
            string id,
            string displayName,
            GameObject[] transparentObjects)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Experiment id cannot be null or empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty.", nameof(displayName));

            Id = id;
            DisplayName = displayName;
            _transparentObjects = transparentObjects;
        }

        public void Activate()
        {
            if (IsActive)
                return;

            if (_transparentObjects == null || _transparentObjects.Length == 0)
            {
                Debug.LogWarning($"[TransparencyExperiment] '{DisplayName}' has no transparent objects assigned. Skipping activation.");
                return;
            }

            if (!_hasStoredOriginalStates)
            {
                _originalActiveStates = new bool[_transparentObjects.Length];
                for (int i = 0; i < _transparentObjects.Length; i++)
                {
                    if (_transparentObjects[i] != null)
                        _originalActiveStates[i] = _transparentObjects[i].activeSelf;
                }
                _hasStoredOriginalStates = true;
            }

            // Enable all transparent objects to maximize overdraw
            int enabledCount = 0;
            for (int i = 0; i < _transparentObjects.Length; i++)
            {
                if (_transparentObjects[i] != null)
                {
                    _transparentObjects[i].SetActive(true);
                    enabledCount++;
                }
            }

            IsActive = true;

            Debug.Log($"[TransparencyExperiment] Activated '{DisplayName}' -> Enabled {enabledCount} transparent objects (max overdraw)");
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            if (_transparentObjects == null || _transparentObjects.Length == 0)
            {
                Debug.LogWarning($"[TransparencyExperiment] '{DisplayName}' has no transparent objects assigned. Skipping deactivation.");
                return;
            }

            if (_hasStoredOriginalStates)
            {
                // Restore original active states
                for (int i = 0; i < _transparentObjects.Length; i++)
                {
                    if (_transparentObjects[i] != null && i < _originalActiveStates.Length)
                        _transparentObjects[i].SetActive(_originalActiveStates[i]);
                }
            }

            IsActive = false;

            Debug.Log($"[TransparencyExperiment] Deactivated '{DisplayName}' -> Restored original object states");
        }
    }
}
