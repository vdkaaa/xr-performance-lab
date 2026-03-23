using System;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Experiments.Transparency
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

        public string Id { get; }
        public string DisplayName { get; }

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

        public void Setup()
        {
            if (_transparentObjects == null || _transparentObjects.Length == 0)
            {
                Debug.LogWarning($"[TransparencyExperiment] Setup '{DisplayName}' -> No transparent objects assigned.");
                return;
            }

            _originalActiveStates = new bool[_transparentObjects.Length];
            for (int i = 0; i < _transparentObjects.Length; i++)
            {
                if (_transparentObjects[i] != null)
                    _originalActiveStates[i] = _transparentObjects[i].activeSelf;
            }

            Debug.Log($"[TransparencyExperiment] Setup '{DisplayName}' -> Saved {_transparentObjects.Length} object states");
        }

        public void Run()
        {
            if (_transparentObjects == null || _transparentObjects.Length == 0)
            {
                Debug.LogWarning($"[TransparencyExperiment] Run '{DisplayName}' -> No transparent objects assigned.");
                return;
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

            Debug.Log($"[TransparencyExperiment] Run '{DisplayName}' -> Enabled {enabledCount} transparent objects (max overdraw)");
        }

        public void Teardown()
        {
            if (_transparentObjects == null || _transparentObjects.Length == 0)
            {
                Debug.LogWarning($"[TransparencyExperiment] Teardown '{DisplayName}' -> No transparent objects assigned.");
                return;
            }

            // Restore original active states
            for (int i = 0; i < _transparentObjects.Length; i++)
            {
                if (_transparentObjects[i] != null && _originalActiveStates != null && i < _originalActiveStates.Length)
                    _transparentObjects[i].SetActive(_originalActiveStates[i]);
            }

            Debug.Log($"[TransparencyExperiment] Teardown '{DisplayName}' -> Restored original object states");
        }
    }
}
