# XR Performance Lab

A modular Unity XR lab designed to study rendering pipelines, profiling workflows, and performance optimization techniques for both PCVR and standalone VR devices such as Meta Quest.

---

# Overview

XR Performance Lab is a technical sandbox built to explore how XR applications behave under different rendering and performance conditions.

Instead of focusing on gameplay, the goal of this project is to create a controlled environment for experimenting with and understanding real-time performance in XR systems.

The lab allows developers to observe and measure how specific variables affect performance.

Examples include:

- Render Scale
- MSAA
- Shadows
- Transparency / Overdraw
- GPU Instancing
- CPU stress scenarios

---

# Motivation

XR applications operate under strict performance constraints.

Maintaining stable framerate targets (72, 80, 90+ FPS depending on device) requires understanding the interaction between:

- CPU workload
- Render thread behavior
- GPU workload
- XR runtime pipelines
- rendering settings and scene complexity

This project exists as a structured environment to study those interactions and document the results.

---

# Technical Focus

This repository focuses on the following XR engineering topics:

- CPU vs GPU bottleneck analysis
- Render thread behavior
- Draw calls, batching and GPU instancing
- Fill rate and render scale impact
- Shadow cost in XR
- Transparency and overdraw
- Profiling workflows in Unity

The goal is to run reproducible experiments and document their results.

---

# Architecture Principles

The project is built following clean and modular architecture principles.

Key design goals include:

- clear separation of responsibilities
- interface-driven systems
- modular experiment modules
- reproducible test scenarios
- maintainable and scalable structure

The system is organized so that new experiments can be added without modifying core systems.

---

# Project Structure

---
Assets/XRPerformanceLab/
  Core/
  Experiments/
  Metrics/
  UI/
  Scenes/
  Prefabs/
  Materials/
  Docs/

---

# Planned Features

The laboratory will include:

- modular experiment runner
- performance metrics overlay
- XR-ready testing scene
- experiment modules affecting rendering variables
- profiling documentation
- PCVR vs Quest standalone comparisons

---

# Repository Status

This repository is currently in active development.

Initial milestones include:

- base architecture
- experiment system
- performance metrics overlay
- first set of rendering experiments

---

# Author

Diego Santander Sepúlveda