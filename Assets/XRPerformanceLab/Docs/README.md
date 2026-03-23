# XR Performance Lab Documentation

## Overview

This folder contains comprehensive documentation for the XR Performance Lab system.

---

## Documents

### [ARCHITECTURE.md](ARCHITECTURE.md)
**Complete system architecture overview**

- Layer diagram (UI → Core → Experiments → Unity APIs)
- Core components (IExperiment, IExperimentRunner, ExperimentRunner, XRLabBootstrap, PlayModeValidator)
- Experiment implementations (MSAA, CPU Throttle, Render Scale, Shadows, Transparency)
- Design principles and patterns
- Configuration reference
- Extensibility guide for adding new experiments

**Read this to understand:** What components exist, how they relate, and the design philosophy.

---

### [SYSTEM_FLOW.md](SYSTEM_FLOW.md)
**Complete system execution flow and state diagrams**

- Initialization flow (at game startup)
- Experiment execution flow (user clicks button → setup → run → hold → teardown)
- Per-frame update during experiment execution
- State machine diagram
- Event flow and subscription model
- Error handling scenarios
- Thread safety and concurrency prevention
- Memory management and lifetime
- Data flow diagrams

**Read this to understand:** How data flows through the system, when things happen, and what state changes occur.

---

## Quick Reference

### Key Concepts

**IExperiment Interface**
\`\`\`csharp
public interface IExperiment
{
    string Id { get; }
    string DisplayName { get; }
    void Setup();      // Save original state
    void Run();        // Apply test condition
    void Teardown();   // Restore state
}
\`\`\`

**For more details, see [ARCHITECTURE.md](ARCHITECTURE.md) and [SYSTEM_FLOW.md](SYSTEM_FLOW.md)**
