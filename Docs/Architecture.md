XR Performance Lab – Architecture Overview

Introduction

XR Performance Lab is designed as a modular experimentation environment for studying rendering behavior and performance in Unity XR applications.

The architecture focuses on clarity, scalability, and reproducibility.
Each system has a well-defined responsibility and communicates through interfaces to reduce coupling.


Core Design Principles

The project follows these key principles:

- Single Responsibility Principle
  Each module has one clear purpose.

- Open/Closed Principle
  New experiments can be added without modifying existing systems.

- Interface-driven design
  Systems depend on abstractions instead of concrete implementations.

- Modularity
  Features are composed as independent modules rather than hardcoded logic.

- Reproducibility
  Experiments are designed to isolate variables and produce consistent results.


High-Level Architecture

The system is divided into four main layers:

1. Core

The Core layer defines the contracts and orchestration logic of the system.

Responsibilities:
- Define experiment interfaces
- Register available experiments
- Execute and control experiments

Key components:
- IExperiment
- IExperimentRegistry
- IExperimentRunner

This layer does not depend on specific experiment implementations.


2. Experiments

The Experiments layer contains isolated modules that modify a single performance variable.

Examples:
- Render Scale
- MSAA
- Shadows
- Transparency / Overdraw
- CPU Stress

Each experiment implements the IExperiment interface and can be activated or deactivated independently.

Design rule:
Each experiment should modify only one primary variable to keep results interpretable.


3. Metrics

The Metrics layer provides runtime performance data.

Responsibilities:
- Gather performance metrics
- Expose data through a unified model

Key components:
- IMetricProvider
- PerformanceMetrics

Metrics may include:
- FPS
- Frame time
- Draw calls
- Batches
- Triangles

This layer is fully decoupled from UI and experiments.


4. UI

The UI layer is responsible only for presentation and interaction.

Responsibilities:
- Display metrics
- Trigger experiments
- Provide debug controls

Key components:
- ILabPanelView
- LabPanelController (planned)

Important:
The UI does not calculate metrics or contain experiment logic.


System Flow

The system is designed around a clear flow of control:

UI (Lab Panel)
    ↓
Experiment Runner
    ↓
Experiment Registry
    ↓
Experiments

And separately:

Metric Provider
    ↓
UI (Display Only)

This separation ensures:
- UI remains lightweight
- Experiments remain independent
- Metrics remain reusable


Dependency Strategy

The system follows dependency inversion:

- High-level modules depend on interfaces
- Low-level implementations are registered or injected

This allows:
- easier testing
- safer refactoring
- flexible extension


Scalability Approach

New experiments can be added by:

1. Creating a new class implementing IExperiment
2. Registering it in the experiment registry

No changes are required in:
- UI
- runner
- existing experiments


Design Intent

This architecture is intentionally simple but extensible.

The goal is to create:
- a clean experimentation environment
- a reproducible profiling workflow
- a maintainable codebase


Summary

XR Performance Lab is structured to separate concerns between:

- experiment logic
- execution control
- performance measurement
- visualization

This allows the project to scale through composition rather than complexity.