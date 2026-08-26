# Packaging the NORSOK Joint Calculator

Builds the python prototype in `../python_prototype` into a single `.exe`, so it can be handed to
someone who has IDEA StatiCa but no Python. Nothing here is part of the app — the prototype stays
a plain runnable python app, and everything to do with packaging lives in this folder.

Why the prototype is being shipped at all: [`../../PYTHON_STOPGAP.md`](../../PYTHON_STOPGAP.md).

**`README_CUSTOMER.md` is the file that goes with the exe.** Keep it free of anything internal —
build steps, repository layout, the stopgap rationale, known defects of the C# app. This file is
the internal half.

## Build

Once, on the build machine:

```
py -m pip install pyinstaller
py -m pip install -r ../python_prototype/requirements.txt
```

Then, from **this** folder:

```
py -m PyInstaller norsok_calculator.spec --noconfirm
```

Result: `dist/NorsokJointCalculator.exe` — one file, ~25 MB (mostly numpy).

## What to hand over

`dist/NorsokJointCalculator.exe` plus `README_CUSTOMER.md`, renamed to `README.md` beside it.
Nothing else: `ui.html`, `lib/` (MathJax, Three.js) and every python module are inside the exe.

The IDEA StatiCa REST service is deliberately **not** bundled — the app locates an installed one
at run time (`service_exe` in `app.py`: `IDEA_CONNECTION_REST_EXE` → the registry's
`CurrentInstallDir` → `C:\Program Files\IDEA StatiCa`), preferring 26.0 and refusing anything
below it.

## Why one file

Measured against the one-dir alternative:

| | one-dir | one-file |
|---|---|---|
| what travels | 57 MB folder; the exe alone does not run | **25 MB, one file** |
| launch | immediate | **~5 s** (unpacks to temp each time) |
| `norsok_app.log` | beside the exe | **beside the exe** |

The 5 s is immaterial next to starting the REST service, and one file removes the "do not take the
exe out of the folder" trap.

The usual one-file objection — the log disappearing into the temp folder — does not apply, because
`app.py` separates `BUNDLE_DIR` (`sys._MEIPASS`, read-only bundled files) from `DATA_DIR`
(`sys.executable`'s folder, what we write). Verified on a built exe: the log lands beside it.

## Not leaking a licence seat

A service the app starts holds an IDEA StatiCa licence seat, and the window's `closed` event does
not fire on a crash or a Task Manager kill. That leak also compounds: the app starts its service on
a free port but only probes 5000 for a foreign one, so the next launch never finds the orphan and
starts another. The single-instance mutex does not help — the orphan is not an instance of the app.

`app.py` therefore uses three mechanisms, each covering what the others cannot:

| | covers | fails on |
|---|---|---|
| **Job Object**, `JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE` | a hard kill of the app — Windows itself kills the job's processes when the last handle closes | nothing, when the job could be created |
| `atexit` | an orderly interpreter exit that never reaches the window event | a hard kill |
| PID file in `DATA_DIR`, reaped at startup | whatever still gets through, on the *next* launch | the seat stays held until then |

The Job Object is built with `ctypes` against `kernel32` (`CreateJobObjectW` →
`SetInformationJobObject` → `AssignProcessToJobObject`), so it adds no dependency. Every step
degrades to "no job" and logs a warning rather than blocking startup — the other two mechanisms
remain.

The service is assigned to the job **before** the 30 s wait for it to answer, so a service that
hangs during startup is covered too.

Two details that matter:

- the PID file goes in `DATA_DIR`, never `BUNDLE_DIR` — in a one-file build `BUNDLE_DIR` is the
  `_MEI…` temp unpack, a different folder on every launch, so a PID written there could never be
  read back;
- reaping verifies the recorded PID's **image name** is `IdeaStatiCa.ConnectionRestApi.exe` before
  terminating it. PIDs are recycled, and killing an unrelated process would be far worse than
  leaking a seat.

## Logging must never block startup

`RotatingFileHandler` on `DATA_DIR` throws whenever the exe sits somewhere unwritable (Program
Files, a read-only share, run from inside the ZIP). With `console=False` that surfaced as nothing
at all: exit code 1, the traceback to a stderr no one sees, and a double-click that appears to do
nothing.

`_init_logging` now falls back to `%LOCALAPPDATA%\NorsokJointCalculator\`, and to no file logging
at all if even that fails, reporting the outcome in a message box. `LOG_PATH` is `None` in that
last case — anything interpolating it into a user-facing string has to handle that.

## Temp folders

One-file means each launch unpacks ~50 MB into `%TEMP%\_MEI…`. The bootloader removes it on a
clean exit and cannot on a kill, so a killed run leaves it behind. Measured on this build: 49.5 MB
left after `TerminateProcess`. This is bootloader behaviour, not something app code can fix — it is
documented for the recipient in `README_CUSTOMER.md` and deliberately not worked around here.

## Notes

- `n63.py` (chapter 6.3, tubular members) is not wired into the app but is re-exported by
  `norsok/__init__.py`, so it is bundled. Left alone deliberately: 12 kB, and excluding it would
  break the package import.
- `console=False` — a stray console window would be noise; diagnostics go to the log instead.
- `dist/` and `build/` are git-ignored.
