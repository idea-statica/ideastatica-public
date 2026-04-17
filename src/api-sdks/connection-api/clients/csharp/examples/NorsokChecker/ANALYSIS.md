# Norsok Checker — Analysis & Implementation Plan

## Overview

A WPF desktop application that evaluates steel connection designs against **NORSOK M-001** (Structural Steel Design) requirements using the IDEA StatiCa Connection API. The tool opens `.ideaCon` project files, runs CBFEM analysis, and applies Norsok-specific acceptance criteria on top of the standard IDEA StatiCa results.

## Norsok M-001 — Key Requirements for Connection Design

NORSOK M-001 (Ed. 6, 2020) references **Eurocode 3** (EN 1993-1-8) as the primary basis for connection design but adds additional requirements and restrictions specific to offshore steel structures.

### Critical Norsok Additions Beyond Eurocode

| Area | Norsok Requirement | Reference |
|---|---|---|
| **Material toughness** | Charpy V-notch requirements at design temperature; material selection per NORSOK M-120 | M-001 §5.1 |
| **Fatigue** | All connections must be fatigue-checked if exposed to cyclic loading; S-N curves per DNVGL-RP-C203 | M-001 §6.4 |
| **Corrosion allowance** | Structural members in splash zone: 2 mm corrosion allowance on all exposed surfaces | M-001 §5.4 |
| **Bolt grades** | Only 8.8 and 10.9 grade bolts; no use of fitted bolts in primary structures without approval | M-001 §9.3 |
| **Weld quality** | Minimum weld quality level B per ISO 5817 for primary structural welds; full penetration welds preferred at critical connections | M-001 §9.2 |
| **Utilization limits** | Connection utilization ≤ 1.0 under ULS; additional checks for ALS (Accidental Limit State) | M-001 §6 |
| **Partial safety factors** | γM0=1.05, γM1=1.10, γM2=1.25 (more conservative than standard EC3) | M-001 §6.1 |
| **Robustness** | Connections shall be designed to avoid brittle failure; ductility requirements per clause 6.5 | M-001 §6.5 |
| **Inspection** | Connection categorization for NDT requirements (Category A/B/C) | M-001 §10.4 |

### Norsok-Specific Checks That Augment CBFEM Results

1. **Utilization ratio validation** — Confirm max utilization ≤ 1.0 for all components (plates, welds, bolts) under all load cases
2. **Bolt grade compliance** — Verify bolt assemblies use 8.8 or 10.9 grade only
3. **Weld type checks** — Flag fillet welds where full-penetration butt welds are required (primary load-bearing connections)
4. **Plate thickness checks** — Validate corrosion allowance is applied where required
5. **Ductility assessment** — Evaluate if connection behavior shows sufficient plastic rotation capacity
6. **Load combination completeness** — Verify ULS, SLS, and ALS combinations are all present
7. **Partial factor verification** — Confirm partial safety factors match Norsok requirements

## Implementation Phases

### Phase 1: Foundation (Current — v0.1) ✅
- [x] WPF project scaffold with Material Design
- [x] Connection API integration (spawn / attach modes)
- [x] Project loading and connection discovery
- [x] Basic CBFEM calculation run
- [x] Max utilization extraction from results
- [x] Simple PASS / FAIL per connection

### Phase 2: Norsok Rule Engine (v0.2)
- [ ] Create `NorsokRuleEngine` service with pluggable rule system
- [ ] Implement Rule: **Utilization limit check** (per component type: plates, welds, bolts)
- [ ] Implement Rule: **Bolt grade validation** (read bolt assembly data from API, check grade)
- [ ] Implement Rule: **Weld quality classification** (full-pen vs. fillet, flag warnings)
- [ ] Per-rule PASS/FAIL/WARNING status with structured detail messages
- [ ] Aggregate results into a `NorsokComplianceReport` model

### Phase 3: Results & Visualization (v0.3)
- [ ] Results tab: DataGrid with per-rule breakdown per connection
- [ ] Color-coded status (green=PASS, red=FAIL, yellow=WARNING)
- [ ] Connection-level summary cards (max utilization, critical bolt, critical weld)
- [ ] Load case breakdown view
- [ ] Export results to JSON

### Phase 4: Reporting (v0.4)
- [ ] Generate structured Norsok compliance report
- [ ] PDF export (or HTML-based report)
- [ ] Include: project info, connection summary, per-connection detail, overall verdict
- [ ] Configurable report template

### Phase 5: Advanced Checks (v0.5+)
- [ ] Fatigue check integration (if API supports fatigue results)
- [ ] Corrosion allowance validation (parameter-based thickness checks)
- [ ] Partial safety factor verification (read from code setup)
- [ ] ALS load case identification and separate evaluation
- [ ] NDT category classification based on connection geometry and loading
- [ ] Ductility classification (brittle vs. ductile failure mode detection)

## Architecture

```
NorsokChecker/
├── App.xaml(.cs)                  # Entry point, Material Design theme
├── MainWindow.xaml(.cs)           # Main shell: config, tabs, log
├── Models/
│   ├── ConnectionCheckResult.cs   # UI-bound connection row model
│   └── NorsokResult.cs            # Check result data model
├── Services/
│   ├── NorsokCheckRunner.cs       # CBFEM calculation + result extraction
│   ├── NorsokRuleEngine.cs        # (Phase 2) Pluggable rule evaluation
│   └── Rules/                     # (Phase 2) Individual Norsok rules
│       ├── INorsokRule.cs
│       ├── UtilizationLimitRule.cs
│       ├── BoltGradeRule.cs
│       └── WeldQualityRule.cs
├── ViewModels/                    # (Phase 3) MVVM ViewModels
├── Views/                         # (Phase 3) Additional views
├── NuGet.config
├── NorsokChecker.csproj
├── NorsokChecker.sln
└── ANALYSIS.md                    # This file
```

## Connection API Endpoints Used

| Endpoint | Purpose |
|---|---|
| `Project.OpenProjectAsync(path)` | Load .ideaCon file, get project ID and connections |
| `Project.CloseProjectAsync(id)` | Clean shutdown |
| `Connection.GetConnectionsAsync(projectId)` | List all connections |
| `Calculation.CalculateAsync(projectId, connectionIds)` | Run CBFEM analysis |
| `Calculation.GetResultsAsync(projectId, connectionIds)` | Get detailed check results |
| `Parameter.GetParametersAsync(projectId, connectionId)` | Read connection parameters |
| `Parameter.UpdateAsync(projectId, connectionId, updates)` | Modify parameters (e.g., apply corrosion allowance) |

## Open Questions

1. **Bolt assembly data** — Does the API expose bolt grade/size per assembly? Need to verify `BoltAssembly` endpoints.
2. **Weld type detail** — Can we distinguish fillet vs. butt weld from API results, or only from geometry?
3. **Partial safety factors** — Are these readable from the project/code setup, or hardcoded per design code?
4. **Fatigue results** — Does the current API return fatigue-related data, or is this only available via dedicated fatigue module?
5. **ALS load cases** — How are accidental load cases identified in the IOM / load effect structure?

## References

- NORSOK M-001 Ed. 6, 2020 — Structural Steel Design
- NORSOK M-120 — Material Data Sheets for Structural Steel
- EN 1993-1-8 — Design of steel structures: Design of joints
- DNVGL-RP-C203 — Fatigue design of offshore steel structures
- ISO 5817 — Welding: Fusion-welded joints in steel, quality levels
