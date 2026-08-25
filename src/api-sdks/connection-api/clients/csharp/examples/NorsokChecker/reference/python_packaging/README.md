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

## Notes

- `n63.py` (chapter 6.3, tubular members) is not wired into the app but is re-exported by
  `norsok/__init__.py`, so it is bundled. Left alone deliberately: 12 kB, and excluding it would
  break the package import.
- `console=False` — a stray console window would be noise; diagnostics go to the log instead.
- `dist/` and `build/` are git-ignored.
