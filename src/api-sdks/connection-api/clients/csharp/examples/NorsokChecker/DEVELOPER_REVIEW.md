# NorsokChecker — request for a software review

To: Tomáš Kohoutek. From: Ondřej Skorunka. 2026-09-06.

## Why you, and what for

You wrote the app and the §6.4 port; since 2026-08-25 I have put 166 commits on top of it, and I am
not a programmer. Most of that code was written with Claude Code under my direction: I decided what
the app should do and read every screen and every report, but I cannot judge the software itself.
This is a request to look at it with a developer's eyes. **Not** at the correctness of the checks —
that side has its oracles (`UNIFICATION.md`, `reference/README.md`) and three external review rounds
behind it — but at everything else:

1. **Did anything from the internal monorepo or my machine leak into a public repository?** Paths,
   feeds, work-item numbers, customer data, credentials, my local folder layout, internal notes.
2. **Is it sound software?** Process and service lifecycle, threading, static state, error handling,
   what is written into the user's project, what leaves the machine.
3. **Is the shape defensible?** File sizes, partial classes, dead and mothballed code, the test
   project, what the CI does with it.
4. **Anything a reviewer of a public example would object to** — this lives under `examples/` of
   `ideastatica-public`, so it is read as IDEA StatiCa's recommended usage of the Connection API.

## What to review

Branch `feature/norsok-checker` at `296f4608`. The range is **`35e8d70a..296f4608`** — from your
`UNIFICATION.md` commit to today — restricted to the two folders:

```
git diff --stat 35e8d70a 296f4608 -- src/api-sdks/connection-api/clients/csharp/examples/NorsokChecker src/api-sdks/connection-api/clients/csharp/examples/UT_NorsokChecker
```

125 files, +19 588 / −2 935 lines, plus 30 MB of IDEA auto-save binaries removed. 168 commits in the
range, one sentence each; the log of the range over the two folders lists them.

Two of the commits in the range are yours (`9d3bc023` NuGet, `dc11192d` Diagnostics); everything else
is mine. Nothing outside the two folders was touched by this work.

## Things I already know about and want your verdict on

Listed so you do not have to discover them; each is a question, not a defence.

**Public-repository hygiene**

- `NorsokChecker.csproj` references `..\..\..\..\..\..\..\..\Common\IdeaStatiCa.Diagnostics\…` — a
  path that leaves the public repository into the surrounding monorepo (your `dc11192d`). The
  comment says so. It means the example does not build from a clone of `ideastatica-public` alone.
  Is that acceptable for an example, or should Diagnostics become optional?
- `App.xaml.cs` hardcodes the Sentry DSN of `desktop_con_norsokchecker`. The comment argues a DSN
  is submit-only and every IDEA app does the same. Please confirm.
- Tracked binaries: the benchmark `.ideaCon` models are synthetic verification joints, not customer
  models; `test_cs.ideaCon` (340 kB) I built for gate coverage. The IDEA autosave `.Archiv`/`.backup`
  files (30 MB) and the review `.docx` that came in with the verification scripts were removed in
  `900871eb` and are now gitignored — **but the branch history still carries them**. Whether that
  warrants rewriting the history of a public branch is your call.
- Working notes in the project root: `ANALYSIS.md`, `BENCHMARK.md`, `BRIEF_REPORT.md`,
  `CHAPTER_63_FINDINGS.md`, `CHAPTER_63_REVISIT.md`, `PYTHON_STOPGAP.md`, `UNIFICATION.md`. They are
  internal process documents; the passages that named colleagues, my local folders or delivery
  circumstances were reworded in `900871eb`. Keep, move to `docs/`, or trim further?
- `NuGet.config` in the project clears all sources and adds `nuget.org` only — deliberately, so no
  internal feed is ever used. Please confirm that is what you want here.
- Vendored third-party code: KaTeX 0.16.11 in `Resources/` (MIT, `LICENSE.katex.txt`,
  `THIRD_PARTY_NOTICES.md` records the one modification — fonts inlined as data: URIs);
  MathJax 3.2.2 and three.js r160 under `reference/python_prototype/lib/` with their licences. Is the
  notice convention right for this repo?

**Process and lifecycle**

- `Services/ServiceReaper.cs` puts the spawned REST service into a **Windows Job Object** via
  `kernel32` P/Invoke so a crash of the app cannot leak a licence seat. I do not know the
  conventions for P/Invoke in this codebase.
- `Services/ServiceLocator.cs` reads `HKLM\SOFTWARE\IDEAStatiCa\<ver>\IDEAStatiCa\Designer\InstallDir64`
  to enumerate installed versions, and pins the service to **26.0.x** because
  `IdeaStatiCa.ConnectionApi 26.0.4.1707` deserialises the 26.1 IOM export to null. Is a client
  upgrade planned, and is the registry path the one to rely on?
- The WebView2 user-data folder lives under `%LOCALAPPDATA%\IdeaStatiCa\NorsokChecker`
  (`Services/WebViewEnvironment.cs`), with a fallback to WebView2's default when it cannot be used.
  There is **no single-instance guard** in the C# app (the python stopgap has one); two instances
  spawning two services is possible.
- `ProjectSettingsService` **writes γ_M0/M1 = 1.15 and γ_M2 = 1.30 into the user's project settings**
  before a run (the report discloses it on its first page). The app modifies the model it reads.
  Is that acceptable behaviour for a tool, or should it be opt-in?

**Threading and state**

- Several `async void` handlers on `MainWindow` (WPF events); `PopulateReportTab` has a
  re-entrancy guard because it is called from three places. Please look for others that need one.
- Static display state: `ConnectionCheckResult.Display` (serves a WPF binding) and
  `NorsokHtmlReportGenerator._pctDecimals`. A test (`NoCalculationTypeHoldsDisplayState`) forbids any
  such static under `Services.Norsok64` and `Services.Chapters`; the two above are outside it on
  purpose. Tell me if that boundary is the wrong one.
- `NorsokHtmlReportGenerator.cs` is **3 314 lines**, including a ~500-line CSS constant and the
  embedded-KaTeX plumbing. `MainWindow` is split into `Api` / `CheckTab` / `Run` / `Results` /
  `Report` / `Joint64` partials sharing private caches (`ANALYSIS.md` records why). Both are known
  debts; I would like your view on whether they block anything.

**Mothballed code**

- `Services/Formulas63_Mothballed/` and `Services/Cbfem_Mothballed/` are **still compiled** (no
  `Compile Remove` in the csproj). They have no callers except each other. Delete, exclude, or keep?

**Tests and CI**

- `UT_NorsokChecker`: 125 tests, all offline except `LiveValidationTests` (`[Explicit]`, needs a
  local IDEA StatiCa installation) and the `Probe`-category fixtures that write to `%TEMP%`.
  `InternalsVisibleTo("UT_NorsokChecker")` on the app. Does the repository's GitHub workflow build
  or run any of this, and should it?
- `UT_NorsokChecker/TestData/*.py` are the oracle generators (need the python prototype). Fine to
  keep beside the tests?

## Findings of an automated sweep of the two folders

A read-only sweep over the 151 tracked files (135 + 16) for internal URLs, work items, secrets,
local paths, names, binaries and private notes. I re-checked every hit listed here against the file
before writing it down. **Clean:** no Azure DevOps or internal feed URL, no work-item / PR / pipeline
number, no PAT, key or e-mail address, no `C:\Users\…` path inside the two folders, no TODO/HACK/
FIXME, no commented-out code, `bin/`, `obj/`, `dist/`, `build/`, `__pycache__/` all ignored, every
PackageReference resolves on nuget.org, the `.ideaCon` models are opaque (no author or path inside)
and demonstrably synthetic.

What it did find, in the order I would act on it:

| # | Where | What | Suggested action |
|---|---|---|---|
| 1 | `reference/verification_scripts/**` | 9 IDEA auto-save files: `*.ideaCon.Archiv` (14.1 + 8.5 + 7.0 MB) and `*.ideaCon.backup` — 30.3 MB, **96 % of the folder's weight**, referenced by nothing | **done** `900871eb`: deleted and gitignored; the history still carries them — a rewrite is your decision |
| 2 | `NorsokChecker.csproj` | `ProjectReference` eight `..\` up into the monorepo (`IdeaStatiCa.Diagnostics`) — the public example does not compile from a public clone, and the package bypasses the `nuget.org`-only `NuGet.config` | publish the package, or guard the telemetry path with a build condition |
| 3 | `…/NORSOK_TY_CONNECTIONS_UNIT_TESTS/TY_JOINT_NORSOK_ISSUES.docx` | 173 kB binary; OOXML metadata with a colleague's name; body is one internal review question in Czech, already answered in the signs note | **done** `900871eb`: removed |
| 4 | `PYTHON_STOPGAP.md:12–15`, `BRIEF_REPORT.md:89–95`, `UNIFICATION.md:117` | internal-audience text now public: a customer delivery at risk and "remaining defects" of the C# app; our QA process and that an AI agent is the last outside reviewer; an open task assigned to a named colleague | **done** `900871eb`: the passages reworded for the audience; the documents stay |
| 5 | `App.xaml.cs:16–17` | live Sentry DSN (submit-only, but harvestable for quota abuse from a public sample) | your call — configuration/env would keep the pattern and drop the exposure |
| 6 | `Services/*_Mothballed/` | compiled into the shipped assembly; the namespaces do not follow the folders (`RawResultsParser.cs` → `NorsokChecker.Services`, all `Formulas63_*` → `NorsokChecker.Services.Formulas`), so "mothballed" is a folder name only | `Compile Remove`, or a branch/tag plus the two READMEs |
| 7 | `UT_NorsokChecker/Norsok64EngineTests.cs:63, 98, 142` | test methods named after a colleague; `reference/README.md:9` attributes by name too | **done** `900871eb`: `KJoint_MatchesHandCalculation` etc., "the hand verification scripts" |
| 8 | `BRIEF_REPORT.md:90, 139`, `UNIFICATION.md:108` | references to a private folder tree outside the repository | **done** `900871eb` |
| 9 | `reference/verification_scripts/ZNAMENKA_MOMENTU_CHORDU.md` | Czech throughout; cited an unreleased build | **done** `900871eb`: translated as `CHORD_MOMENT_SIGNS.md`, build number dropped |
| 10 | `UT_NorsokChecker/TestData/live_oracle.json:2` | `"service_setup_dir": "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.1"` baked into committed test data | **done** `900871eb`: field dropped from the oracle and its generator |
| 11 | `reference/python_prototype/requirements.txt` | `pywebview`, `requests`, `numpy` unpinned, in a folder whose purpose is reproducibility | pin |

Rows 2 (the `ProjectReference` into the monorepo), 5 (Sentry DSN), 6 (mothballed folders compiled)
and 11 (`requirements.txt` pins) are yours to decide; I did not touch them.

Out of scope, same reviewer: `examples/SweepChecker/Properties/PublishProfiles/FolderProfile.pubxml:7`
holds a `PublishDir` under a user's desktop — a personal path and a project name.

Also seen and judged acceptable, but listed so you can disagree: seven hardcoded `localhost:5000` /
`C:\Program Files\IDEA StatiCa` defaults (all mitigated by the registry/service locator, none
configurable from outside); `NorsokHtmlReportGenerator.cs` at 178 kB in one file; the working notes
`ANALYSIS.md`, `BENCHMARK.md`, `CHAPTER_63_*.md`, `Cbfem_Mothballed/README.md` (candid but not
sensitive); telemetry on by default with no opt-out in a public sample (the privacy design itself is
careful: no paths, names, values or messages leave the machine).

## What I am not asking

Whether the §6.4 numbers are right. That is covered by the oracles and the review rounds, and if
you do spot something there I will take it gladly, but it is not the point of this pass.

## How to answer

Comments in the diff, a list, or a call — whatever is least work. The one thing I need in writing
is the answer to question 1 (leakage), because it decides what has to be scrubbed from the public
history and how.
