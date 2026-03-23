# System Flow - XR Performance Lab

## Overview

This document describes the complete flow of how experiments are registered, discovered, and executed in the XR Performance Lab system.

---

## Initialization Flow (at Game Start)

```
Game Start
    ↓
XRLabBootstrap.Awake()  [DefaultExecutionOrder=-100]
    ↓
[1] DontDestroyOnLoad(gameObject)
    ↓
[2] Get or Create ExperimentRunner MonoBehaviour
    Runner = GetComponent<ExperimentRunner>() || gameObject.AddComponent<ExperimentRunner>()
    ↓
[3] Configure HoldDuration
    Runner.HoldDuration = holdDuration  // from SerializeField
    ↓
[4] Create AllExperiments list
    AllExperiments = new List<IExperiment>()
    ↓
[5] Call RegisterExperiments()
    ├─ Register MSAA instances (0, 2, 4, 8)
    ├─ Register CPU Throttle instances (-1, 30, 45, 72, 90)
    ├─ Register Render Scale instances (0.5, 0.75, 1.0, 1.5)
    ├─ Register Shadow instances (0, 50, 100, 150)
    └─ Register Transparency instance (1)
    ↓
[6] Log initialization
    "[XRLabBootstrap] Initialized with 18 experiments (Hold Duration: 3.0s)"
    ↓
PlayModeValidator.Start()  [if Editor]
    ├─ FindFirstObjectByType<XRLabBootstrap>()
    ├─ Cache _runner = bootstrap.Runner
    ├─ Cache _experiments = bootstrap.AllExperiments.ToArray()
    ├─ Subscribe to OnExperimentCompleted event
    └─ Ready for user interaction
    ↓
✓ System ready for experiment execution
```

---

## Experiment Execution Flow

### User Action: Click Button in PlayModeValidator

```
User clicks "Run Selected" button
    ↓
PlayModeValidator.RunSelectedExperiment()
    ├─ Validate _selectedExperimentIndex
    ├─ Set _currentRunningExperiment = experiment.DisplayName
    ├─ Set _holdTimeRemaining = _runner.HoldDuration  // e.g., 3.0
    ├─ Log: "[PlayModeValidator] Starting experiment: {name} (ID: {id})"
    └─ _bootstrap.StartCoroutine(_runner.RunExperimentRoutine(experiment))
        ↓
        ┌─────────────────────────────────────────────────┐
        │  ExperimentRunner.RunExperimentRoutine()        │
        │  (Coroutine - executes over multiple frames)    │
        └─────────────────────────────────────────────────┘
            ↓
        [Validation]
            if (IsRunning)
            {
                Debug.LogWarning("Another experiment already running");
                yield break;  // Exit early
            }
            ↓
        [Ready to Execute]
            IsRunning = true
            float holdTimeRemaining = HoldDuration  // 3.0 seconds
            ↓
        TRY BLOCK:
        ┌─────────────────────────────────────────────────┐
        │  experiment.Setup()                             │
        │  ├─ Save _originalValue = GetUnityState()       │
        │  └─ Log: "[Experiment] Setup: saved {value}"    │
        │                                                  │
        │  experiment.Run()                               │
        │  ├─ Apply test condition                        │
        │  └─ Log: "[Experiment] Run: applied {value}"    │
        │                                                  │
        │  while (holdTimeRemaining > 0f)                 │
        │  {                                              │
        │      yield return null;  // Wait one frame      │
        │      holdTimeRemaining -= Time.deltaTime;       │
        │      // During this time:                       │
        │      // - PlayModeValidator.Update() ticks down │
        │      // - Metrics can stabilize                 │
        │      // - GUI shows countdown: "(2.4s remaining)"
        │  }                                              │
        │                                                  │
        │  Log: "[ExperimentRunner] Hold complete..."     │
        └─────────────────────────────────────────────────┘
            ↓
        FINALLY BLOCK (always executes):
        ┌─────────────────────────────────────────────────┐
        │  experiment.Teardown()                          │
        │  ├─ UnityAPI.Set(_originalValue)                │
        │  └─ Log: "[Experiment] Teardown: restored"      │
        │                                                  │
        │  IsRunning = false                              │
        │                                                  │
        │  Log: "[ExperimentRunner] Completed {name}"     │
        │                                                  │
        │  OnExperimentCompleted?.Invoke(experimentId)    │
        └─────────────────────────────────────────────────┘
            ↓
        PlayModeValidator.OnExperimentCompleted(experimentId)
            ├─ Find experiment by ID
            ├─ Log: "[Validator] {name} completed"
            ├─ _currentRunningExperiment = "Idle"
            ├─ _holdTimeRemaining = 0f
            └─ Status reverts to "Status: Idle"
                ↓
        ✓ Experiment execution complete
```

---

## Per-Frame Update During Execution

### PlayModeValidator.Update()
```csharp
if (_runner != null && _runner.IsRunning && _holdTimeRemaining > 0f)
{
    _holdTimeRemaining -= Time.deltaTime;  // Countdown
}
```

### PlayModeValidator.OnGUI() (every frame)
```
Display status:
  if IsRunning and _holdTimeRemaining > 0:
      "Running: Experiment Name\n(2.4s remaining)"
  else if IsRunning:
      "Running: Experiment Name"
  else:
      "Status: Idle"

Display UI button state:
  if IsRunning:
      "Run Selected" button DISABLED
      "Running..." indicator shown
  else:
      "Run Selected" button ENABLED
```

---

## State Machine

```
                    ┌──────────────┐
                    │    IDLE      │
                    │ IsRunning=F  │
                    └──────────────┘
                           ▲
                           │
              [User clicks Run Selected]
                           │
                           ▼
                    ┌──────────────┐
                    │   SETUP      │
                    │ IsRunning=T  │
                    └──────────────┘
                           │
                    [experiment.Setup()]
                           │
                           ▼
                    ┌──────────────┐
                    │   RUNNING    │
                    │ IsRunning=T  │
                    └──────────────┘
                           │
                    [experiment.Run()]
                           │
                           ▼
                    ┌──────────────┐
                    │    HOLDING   │
                    │ IsRunning=T  │
                    │ Hold timer   │
                    │ counting     │
                    │ down         │
                    └──────────────┘
                           │
                    [Hold duration = 0]
                           │
                           ▼
                    ┌──────────────┐
                    │  TEARDOWN    │
                    │ IsRunning=T  │
                    │ (finally)    │
                    └──────────────┘
                           │
                    [experiment.Teardown()]
                    [IsRunning = false]
                           │
                           ▼
                    ┌──────────────┐
                    │    IDLE      │
                    │ IsRunning=F  │
                    │ Event fires  │
                    └──────────────┘
```

---

## Thread Safety

**All operations run on the main thread:**

```
Frame N:   Update() runs → _holdTimeRemaining -= Time.deltaTime
Frame N+1: OnGUI() reads _holdTimeRemaining (no race conditions)
Frame N+2: ExperimentRunner coroutine resumes
```

No locks needed. No threading issues.

**Why only main thread?**
- Unity APIs (QualitySettings, renderScale) are main-thread only
- MonoBehaviours always run on main thread
- Coroutines execute on main thread (not async/await)

---

## Event Flow

### Event Subscription (Bootstrap → Runner)
```
ExperimentRunner has:
    public event Action<string> OnExperimentCompleted;

PlayModeValidator subscribes in Start():
    _runner.OnExperimentCompleted += OnExperimentCompleted;
```

### Event Publishing (Runner → Validator)
```
ExperimentRunner (finally block):
    OnExperimentCompleted?.Invoke(experiment.Id);
        ↓
    Calls PlayModeValidator.OnExperimentCompleted(experimentId)
        ├─ Find experiment by ID
        └─ Log completion
```

### Event Cleanup
```
PlayModeValidator.OnDestroy():
    _runner.OnExperimentCompleted -= OnExperimentCompleted;
```

---

## Error Handling

### Scenario 1: User clicks Run while already running
```
User clicks "Run Selected" (while IsRunning=true)
    ↓
PlayModeValidator.RunSelectedExperiment()
    └─ _bootstrap.StartCoroutine(...)  // Starts coroutine
        ↓
ExperimentRunner.RunExperimentRoutine()
    └─ if (IsRunning) {
           Debug.LogWarning("Cannot run...");
           yield break;  // Exit immediately
       }
```

### Scenario 2: Experiment.Run() throws exception
```
ExperimentRunner.RunExperimentRoutine()
    try {
        experiment.Setup();
        experiment.Run();  // ← Throws exception
        // Never reaches here
    }
    finally {
        experiment.Teardown();  // ← Always executes
        IsRunning = false;      // ← Always executes
    }
```

### Scenario 3: No transparent objects assigned
```
TransparencyExperiment.Setup()
    if (_transparentObjects == null || _transparentObjects.Length == 0)
    {
        Debug.LogWarning("No objects assigned");
        return;  // Graceful degradation, don't crash
    }
    // Continue normally
```

---

## Data Flow Diagram

```
┌─────────────────────────────────────────────────────────┐
│           Unity Game Loop (per frame)                   │
└─────────────────────────────────────────────────────────┘
        ↓
    ┌───┴────┬──────────┬─────────────────┐
    ↓        ↓          ↓                 ↓
  Update() OnGUI()   Coroutine      OnDestroy()
           (rendering)  Resume      (cleanup)

    │        │           │                 │
    └────┬───┴───────┬───┴────────┬────────┘
         ↓           ↓            ↓
    Update counters Read state   Resume
    _holdTimeRemaining            experiment
                                  coroutine


┌─────────────────────────────────────────────────────────┐
│        ExperimentRunner.RunExperimentRoutine()          │
└─────────────────────────────────────────────────────────┘
         │
    ┌────┴──────────────────────┬─────────────────┐
    ↓                           ↓                 ↓
 Setup()                     Run()           WaitForSeconds()
    │                         │                 │
    ├─ Save state             ├─ Apply test     ├─ yield return null
    └─ Notify [log]           └─ Notify [log]   └─ Time.deltaTime


┌─────────────────────────────────────────────────────────┐
│              PlayModeValidator.OnGUI()                  │
└─────────────────────────────────────────────────────────┘
    │
    ├─ Draw title
    ├─ Draw status (reads _holdTimeRemaining)
    ├─ Draw experiment list
    └─ Draw Run button (disabled if IsRunning)
```

---

## Memory Management

### Storage
```
XRLabBootstrap.AllExperiments → List<IExperiment>
    └─ 18 experiment instances
    └─ ~5-10KB total (small objects)

ExperimentRunner
    └─ Event delegate list
    └─ Local variables (reused per coroutine)

PlayModeValidator
    └─ _experiments[] array reference (no copy)
    └─ GUI state (_scrollPosition, etc.)
```

### Lifetime
```
XRLabBootstrap
    └─ Created at boot (Awake)
    └─ Persists via DontDestroyOnLoad()
    └─ Destroyed on app shutdown

ExperimentRunner
    └─ Attached to Bootstrap GameObject
    └─ Same lifetime as Bootstrap

PlayModeValidator
    └─ Attached to Bootstrap GameObject (if in Editor)
    └─ Same lifetime as Bootstrap
    └─ Only active when #if UNITY_EDITOR
```

### Cleanup
```
Bootstrap GameObject destroyed
    ↓
Detaches all components
    ├─ ExperimentRunner.OnDestroy() (no special cleanup needed)
    └─ PlayModeValidator.OnDestroy()
        ├─ Unsubscribe from event
        └─ Clear cached references
```

---

## Concurrency Prevention

**Problem:** User could click Run 100 times per second

**Solution:** IsRunning flag checked at entry
```csharp
if (IsRunning)
{
    Debug.LogWarning("Cannot run...");
    yield break;
}
IsRunning = true;  // Set immediately
```

**Result:** Only 1 experiment runs at a time. Additional requests silently ignored with warning.

---

## For Developers

### Adding a New Experiment Type

1. **Create class in `Experiments/{Name}/`**
   ```csharp
   public sealed class MyExperiment : IExperiment
   {
       public string Id => "my-exp";
       public string DisplayName => "My Experiment";

       public void Setup() { /* save */ }
       public void Run() { /* test */ }
       public void Teardown() { /* restore */ }
   }
   ```

2. **Register in Bootstrap.RegisterExperiments()**
   ```csharp
   var experiment = new MyExperiment(...);
   AllExperiments.Add(experiment);
   ```

3. **Auto-discovered by PlayModeValidator**
   - Bootstrap.AllExperiments list updated
   - PlayModeValidator reads it in Start()
   - Button appears in GUI

### Testing a Single Experiment

```csharp
// In any MonoBehaviour:
var bootstrap = FindObjectOfType<XRLabBootstrap>();
bootstrap.RunExperiment("msaa-4");

// OR with PlayModeValidator:
// Just click the button in the GUI!
```

---

**End of System Flow Documentation**
