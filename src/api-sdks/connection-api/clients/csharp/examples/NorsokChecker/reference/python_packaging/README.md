# Packaging the NORSOK Joint Calculator

Builds the python prototype in `../python_prototype` into a folder with an `.exe`, so it can be
handed to someone who has IDEA StatiCa but no Python. Nothing here is part of the app — the
prototype stays a plain runnable python app, and everything to do with packaging lives in this
folder.

Why this exists at all: see [`../../PYTHON_STOPGAP.md`](../../PYTHON_STOPGAP.md).

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

Result: `dist/NorsokJointCalculator/` — about 57 MB, mostly numpy. Ship the **whole folder**
(zip it); the `.exe` alone will not run.

One-dir, not one-file, deliberately: it starts immediately instead of unpacking on every launch,
the log lands next to the `.exe` where the user can find it, and when something fails the parts
are visible. The cost is that the customer receives a folder rather than a single file.

## What the recipient needs

| | |
|---|---|
| **IDEA StatiCa 26.0 or newer** | The app drives the Connection REST API over `/api/4`, which does not exist before 26.0 — a 25.1 service answers `400 UnsupportedApiVersion` on every `/api/4` route. **26.0 is what the app was verified on** and is preferred if several versions are installed. |
| A licence that allows the REST API | The app starts `IdeaStatiCa.ConnectionRestApi.exe`; without a seat the service starts but calculation fails. |
| Edge WebView2 runtime | Present on current Windows. The UI is a native window rendered by WebView2 (via pywebview), so it cannot be bundled — it is a system component. If the window stays blank, install the [WebView2 evergreen runtime](https://developer.microsoft.com/microsoft-edge/webview2/). |
| Nothing else | No Python, no separate MathJax/Three.js download — `ui.html` and `lib/` are inside the bundle. |

## Running it

Double-click `NorsokJointCalculator.exe`, then open an `.ideaCon`.

The service is handled automatically:

- if one is **already answering** on port 5000, the app uses it and leaves it running on exit —
  it did not start it, so it does not stop it;
- otherwise the app **starts its own** on a free port chosen by the OS (passed to the service with
  its `-port=` switch), and shuts it down when the window closes. A busy port 5000 is therefore
  not a problem.

Where the service is found, in order:

1. `IDEA_CONNECTION_REST_EXE` — full path to `IdeaStatiCa.ConnectionRestApi.exe`, if set;
2. the install directory IDEA records in `HKLM\SOFTWARE\IDEA StatiCa\CurrentInstallDir` — this is
   what makes a **non-default install location** work;
3. `C:\Program Files\IDEA StatiCa\StatiCa <version>`.

Across everything found, 26.0 wins; otherwise the newest, with a note that the verified version
is absent. Anything below 26.0 is refused with the reason stated.

## When something goes wrong

**`norsok_app.log`, next to the `.exe`.** It records the normal flow too — which file was opened,
which service and version, how many load effects, how many 6.4 checks passed or were skipped — so
a UI message like "division by zero" is never a dead end. It rotates at ~1 MB × 3 files.

| symptom | cause |
|---|---|
| "No usable IDEA StatiCa installation found" | nothing from 26.0 up was found; the message lists where it looked and what it saw. Set `IDEA_CONNECTION_REST_EXE`. |
| "Service did not come up within 30 s" | the service exists but failed to start — its own start-up problem (licence, .NET runtime). |
| Blank window | WebView2 runtime missing (see above). |
| "No check performed — this joint is outside the scope of NORSOK 6.4" | not a failure: 6.4 covers simple tubular joints, and the page lists the conditions that are unmet. A non-tubular member, a brace off the joint plane, overlapping feet, no through chord. |
| Opening a file fails with a serialization error | the `.ideaCon` was saved by a **newer** version than the service. On `/api/4` the message is unhelpful (`500`), on `/api/3` it names the offending value. Open it in the newer IDEA and save it back, or install that version. |

## Scope

Chapter **6.4** (tubular joints) only. Chapter 6.3 (tubular members) is in the source as
`norsok/n63.py` but is not wired into the app, so no member checks are performed.
