# XR Performance Lab Architecture

## Overview

XR Performance Lab is a modular performance testing system for Unity XR applications. It provides a clean, interface-driven architecture for profiling and benchmarking performance across different settings and conditions.

---

## Architecture Layers

The system follows a **layered architecture** with clear dependency flow:

```
┌─────────────────────────────────────────┐
│             UI Layer                    │
│    (PlayModeValidator - Dev Tool)       │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│          Core Layer                     │
│  (ExperimentRunner, XRLabBootstrap)     │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Experiments Layer                  │
│  (IExperiment implementations)          │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Unity APIs & Assets                │
│  (QualitySettings, URP, GameObjects)    │
└─────────────────────────────────────────┘
```

---

## Core Components

### 1. IExperiment Interface
**Location:** `Core/Interfaces/IExperiment.cs`

Defines the contract for all experiments:
```csharp
public interface IExperiment
{
    string Id { get; }
    string DisplayName { get; }
    void Setup();      // Save original state
    void Run();        // Apply test condition
    void Teardown();   // Restore state
}
```

**Responsibility:** Each experiment modifies exactly ONE performance variable.

---

### 2. IExperimentRunner Interface
**Location:** `Core/Interfaces/IExperimentRunner.cs`

Manages experiment lifecycle with guaranteed cleanup:
```csharp
public interface IExperimentRunner
{
    event Action<string> OnExperimentCompleted;
    float HoldDuration { get; set; }
    bool IsRunning { get; }
    IEnumerator RunExperimentRoutine(IExperiment experiment);
}
```

**Responsibility:** Execute experiments in sequence: Setup → Run → Hold → Teardown

---

### 3. ExperimentRunner (MonoBehaviour)
**Location:** `Core/Services/ExperimentRunner.cs`

Implements the runner with:
- **Coroutine support** for time-based execution
- **try/finally guarantee** - Teardown always runs
- **Configurable hold duration** - Allows metrics to stabilize
- **Event notification** - Fires OnExperimentCompleted

**Execution Sequence:**
```
IsRunning = true
    ↓
experiment.Setup()     // Save original state
    ↓
experiment.Run()       // Apply test condition
    ↓
WaitForSeconds(HoldDuration)  // Let metrics stabilize
    ↓
experiment.Teardown()  // Always runs (try/finally)
    ↓
IsRunning = false
OnExperimentCompleted?.Invoke(experimentId)
```

---

### 4. XRLabBootstrap (MonoBehaviour)
**Location:** `Core/XRLabBootstrap.cs`

**Purpose:** Central dependency injection and experiment registration

**Responsibilities:**
- Creates ExperimentRunner component
- Registers all 18 experiments with configurable parameters
- Provides public API for running experiments
- Persists across scenes (DontDestroyOnLoad)
- [DefaultExecutionOrder(-100)] ensures early initialization

**Registered Experiments:**
- **MSAA:** 4 instances (0, 2, 4, 8)
- **CPU Throttle:** 5 instances (-1, 30, 45, 72, 90)
- **Render Scale:** 4 instances (0.5, 0.75, 1.0, 1.5)
- **Shadow Distance:** 4 instances (0, 50, 100, 150)
- **Transparency:** 1 instance
- **Total:** 18 experiments

---

### 5. PlayModeValidator (MonoBehaviour)
**Location:** `Core/Utilities/PlayModeValidator.cs`

**Purpose:** Dev tool for interactive experiment testing in Play Mode

**Features:**
- OnGUI panel at top-right corner
- Scrollable list of all 18 experiments
- Click to select (highlighted in cyan)
- "Run Selected" button to execute
- Real-time countdown timer showing hold duration
- Status display: "Running: {name} (2.4s remaining)"
- **Editor-only** (#if UNITY_EDITOR)

---

## Experiment Implementations

Each experiment follows the same pattern:

### Setup()
```csharp
public void Setup()
{
    _originalValue = UnityAPI.GetCurrentValue();
    Debug.Log($"[Experiment] Saved original: {_originalValue}");
}
```

### Run()
```csharp
public void Run()
{
    UnityAPI.SetValue(_targetValue);
    Debug.Log($"[Experiment] Applied: {_targetValue}");
}
```

### Teardown()
```csharp
public void Teardown()
{
    UnityAPI.SetValue(_originalValue);
    Debug.Log($"[Experiment] Restored: {_originalValue}");
}
```

---

## Experiments

### MSAAExperiment
- **Modifies:** `QualitySettings.antiAliasing`
- **Levels:** 0, 2, 4, 8
- **Tests:** Anti-aliasing quality vs fill rate cost

### CPUThrottleExperiment
- **Modifies:** `Application.targetFrameRate`
- **Values:** -1, 30, 45, 72, 90
- **Tests:** CPU budget constraints

### RenderScaleExperiment
- **Modifies:** `URP Asset.renderScale`
- **Values:** 0.5, 0.75, 1.0, 1.5
- **Tests:** Resolution scaling impact

### ShadowExperiment
- **Modifies:** `URP Asset.shadowDistance`
- **Values:** 0, 50, 100, 150
- **Tests:** Shadow rendering impact

### TransparencyExperiment
- **Modifies:** `GameObject.activeSelf` (array)
- **Tests:** Overdraw and fill rate pressure

---

## Design Principles

### 1. Single Responsibility
Each experiment modifies exactly ONE variable. No side effects.

### 2. Clean Architecture
- **UI** doesn't access experiments directly
- **Experiments** don't know about UI
- Everything communicates through **interfaces**

### 3. Guaranteed Cleanup
Teardown always executes via try/finally, even if Run() throws.

### 4. Metric Stabilization
Hold duration gives metrics time to settle before cleanup. Critical for accurate benchmarking.

### 5. Console Logging
Every operation logged with consistent format:
```
[ComponentName] Operation: details
```

---

## Configuration

All configurable values are SerializeFields on XRLabBootstrap:

| Component | Field | Default | Purpose |
|-----------|-------|---------|---------|
| Pipeline | pipelineAsset | (required) | URP Asset for RenderScale/Shadow tests |
| MSAA | msaaLevels[] | {0,2,4,8} | Anti-aliasing levels |
| CPU | cpuTargetRates[] | {-1,30,45,72,90} | Frame rate targets |
| Render Scale | renderScaleValues[] | {0.5,0.75,1.0,1.5} | Resolution multipliers |
| Shadows | shadowDistanceValues[] | {0,50,100,150} | Shadow distances in meters |
| Transparency | transparentObjects[] | (empty) | GameObjects for overdraw test |
| **Hold Duration** | holdDuration | 3f | Seconds to wait during Run |

---

## Threading & Coroutines

**All operations run on main thread.**

Why?
- Unity APIs (QualitySettings, renderScale, etc.) are main-thread only
- Coroutines execute via MonoBehaviour (main thread)
- No threading complexity needed

**Hold Duration uses frame-based timing:**
```csharp
float holdTimeRemaining = HoldDuration;
while (holdTimeRemaining > 0f)
{
    yield return null;              // Wait one frame
    holdTimeRemaining -= Time.deltaTime;
}
```

---

## Error Handling

### null/Missing References
```csharp
if (experiment == null)
    throw new ArgumentNullException(nameof(experiment));
```

### Concurrent Execution Prevention
```csharp
if (IsRunning)
{
    Debug.LogWarning("Another experiment already running");
    yield break;
}
```

### Missing Configuration
```csharp
if (transparentObjects == null || transparentObjects.Length == 0)
{
    Debug.LogWarning("No objects assigned");
    return;  // Don't throw - graceful degradation
}
```

---

## Future Extensibility

### Adding a New Experiment Type

1. **Create class in `Experiments/{Name}/`:**
```csharp
public sealed class MyExperiment : IExperiment
{
    public string Id { get; }
    public string DisplayName { get; }

    public void Setup() { /* save state */ }
    public void Run() { /* apply condition */ }
    public void Teardown() { /* restore */ }
}
```

2. **Register in XRLabBootstrap.RegisterExperiments():**
```csharp
var experiment = new MyExperiment(id, displayName, targetValue);
AllExperiments.Add(experiment);
```

3. **Expose SerializeField for configuration** (optional)

---

## Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Experiment Class | `[Name]Experiment` | `MSAAExperiment` |
| Experiment ID | `[kebab-case]` | `msaa-4`, `cpu-90` |
| Namespace | XRPerformanceLab.[Layer] | XRPerformanceLab.Experiments |
| File Location | Experiments/[Name]/ | Experiments/MSAA/ |

---

## Git Commit Scopes

```
core:       IExperiment, ExperimentRunner, Bootstrap
experiment: Experiment implementations
metrics:    Performance data collection
ui:         UI Components and panels
docs:       Documentation updates
```

---

**Architecture ensures: Clean, testable, extensible performance profiling for XR applications.**
