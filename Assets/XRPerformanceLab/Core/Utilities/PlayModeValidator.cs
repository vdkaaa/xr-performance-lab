#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using XRPerformanceLab.Core.Interfaces;

namespace XRPerformanceLab.Core.Utilities
{
    /// <summary>
    /// Dev tool for testing all registered experiments in Unity Play Mode.
    /// Provides a GUI panel to select and run experiments individually.
    /// Only available in Unity Editor builds.
    /// </summary>
    [AddComponentMenu("XR Performance Lab/Play Mode Validator")]
    public sealed class PlayModeValidator : MonoBehaviour
    {
        [Header("Dev Tool - Only Active in Editor")]
        [SerializeField] private bool showPanel = true;

        private XRLabBootstrap _bootstrap;
        private IExperimentRunner _runner;
        private IExperiment[] _experiments;
        private int _selectedExperimentIndex = -1;
        private string _currentRunningExperiment = "Idle";
        private float _holdTimeRemaining = 0f;
        private Vector2 _scrollPosition;

        private const float PanelWidth = 280f;
        private const float PanelMargin = 10f;

        private void Start()
        {
            _bootstrap = FindFirstObjectByType<XRLabBootstrap>();

            if (_bootstrap == null)
            {
                Debug.LogError("[PlayModeValidator] XRLabBootstrap not found in scene!");
                enabled = false;
                return;
            }

            _runner = _bootstrap.Runner;
            _experiments = _bootstrap.AllExperiments.ToArray();

            if (_runner != null)
            {
                _runner.OnExperimentCompleted += OnExperimentCompleted;
            }

            Debug.Log($"[PlayModeValidator] Initialized with {_experiments.Length} experiments.");
        }

        private void OnDestroy()
        {
            if (_runner != null)
            {
                _runner.OnExperimentCompleted -= OnExperimentCompleted;
            }
        }

        private void Update()
        {
            // Decrement hold time remaining during experiment execution
            if (_runner != null && _runner.IsRunning && _holdTimeRemaining > 0f)
            {
                _holdTimeRemaining -= Time.deltaTime;
            }
        }

        private void OnExperimentCompleted(string experimentId)
        {
            var experiment = System.Array.Find(_experiments, e => e.Id == experimentId);
            if (experiment != null)
            {
                Debug.Log($"[Validator] {experiment.DisplayName} completed");
            }

            _currentRunningExperiment = "Idle";
            _holdTimeRemaining = 0f;
        }

        private void OnGUI()
        {
            if (!showPanel || _bootstrap == null || _experiments == null)
                return;

            // Panel positioned at top-right corner
            float panelX = Screen.width - PanelWidth - PanelMargin;
            float panelY = PanelMargin;
            float panelHeight = Screen.height - (PanelMargin * 2);

            GUILayout.BeginArea(new Rect(panelX, panelY, PanelWidth, panelHeight), GUI.skin.box);

            // Title
            GUILayout.Label("Experiment Validator", GUI.skin.box);
            GUILayout.Space(5);

            // Status
            string statusText;
            if (_runner != null && _runner.IsRunning)
            {
                if (_holdTimeRemaining > 0f)
                {
                    statusText = $"Running: {_currentRunningExperiment}\n({_holdTimeRemaining:F1}s remaining)";
                }
                else
                {
                    statusText = $"Running: {_currentRunningExperiment}";
                }
            }
            else
            {
                statusText = "Status: Idle";
            }
            GUILayout.Label(statusText);
            GUILayout.Space(5);

            // Scrollable experiment list
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(panelHeight - 150));

            for (int i = 0; i < _experiments.Length; i++)
            {
                var experiment = _experiments[i];

                // Highlight selected experiment
                Color originalColor = GUI.color;
                if (i == _selectedExperimentIndex)
                {
                    GUI.color = Color.cyan;
                }

                if (GUILayout.Button(experiment.DisplayName, GUILayout.Height(30)))
                {
                    _selectedExperimentIndex = i;
                }

                GUI.color = originalColor;
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            // Run Selected button
            GUI.enabled = _selectedExperimentIndex >= 0 && _runner != null && !_runner.IsRunning;

            if (GUILayout.Button("Run Selected", GUILayout.Height(40)))
            {
                RunSelectedExperiment();
            }

            GUI.enabled = true;

            // Show running indicator
            if (_runner != null && _runner.IsRunning)
            {
                GUILayout.Label("Running...", GUI.skin.box);
            }

            GUILayout.EndArea();
        }

        private void RunSelectedExperiment()
        {
            if (_selectedExperimentIndex < 0 || _selectedExperimentIndex >= _experiments.Length)
            {
                Debug.LogWarning("[PlayModeValidator] No valid experiment selected.");
                return;
            }

            var experiment = _experiments[_selectedExperimentIndex];
            _currentRunningExperiment = experiment.DisplayName;
            _holdTimeRemaining = _runner.HoldDuration;

            Debug.Log($"[PlayModeValidator] Starting experiment: {experiment.DisplayName} (ID: {experiment.Id})");

            _bootstrap.StartCoroutine(_runner.RunExperimentRoutine(experiment));
        }
    }
}
#endif
