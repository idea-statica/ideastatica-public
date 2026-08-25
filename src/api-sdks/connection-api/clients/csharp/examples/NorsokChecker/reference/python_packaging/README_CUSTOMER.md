# NORSOK Joint Calculator

Checks steel tubular joints against **NORSOK N-004 Rev. 3, chapter 6.4** (simple tubular joints),
reading the joint geometry and load effects from an IDEA StatiCa Connection project.

Ships as a single `NorsokJointCalculator.exe`. Nothing to install.

## What you need

| | |
|---|---|
| **IDEA StatiCa 26.0 or newer** | The tool drives the IDEA StatiCa Connection REST API. Version 26.0 is the one it was verified against and is used in preference if you have several installed. Earlier versions (25.1 and below) do not provide the interface it needs and are refused with a message. |
| A licence covering the Connection REST API | Without an available seat the service starts but the calculation fails. |
| Microsoft Edge WebView2 runtime | Present on current Windows installations. It renders the tool's window. If the window stays blank, install the [WebView2 evergreen runtime](https://developer.microsoft.com/microsoft-edge/webview2/). |

## Running it

Double-click `NorsokJointCalculator.exe` and open an `.ideaCon` project. The first launch takes a
few seconds longer while the program unpacks itself.

The IDEA StatiCa service is handled for you:

- if one is already running, the tool uses it and leaves it running when you close the window;
- otherwise the tool starts its own on a free port and shuts it down on exit.

If IDEA StatiCa is installed somewhere unusual and the tool cannot find it, set the environment
variable `IDEA_CONNECTION_REST_EXE` to the full path of `IdeaStatiCa.ConnectionRestApi.exe`.

## Reading the result

Each brace is checked against chapter 6.4 per load effect, with the full derivation available for
every check — classification, resistances and the chord-action factor.

**"No check performed — this joint is outside the scope of NORSOK 6.4"** is a result, not a
failure. Chapter 6.4 applies to *simple tubular joints*, and the page lists the conditions that
are not met: a member that is not a circular hollow section, a brace off the joint plane,
overlapping brace feet, no through chord, a brace parallel to the chord.

The whole joint is withheld rather than the offending member alone, deliberately. The joint plane,
the chord stresses averaged across it and the K/Y/X force balance are properties of the joint as a
whole, so once any of those conditions fails, no individual brace can be assessed either — not
even one whose own geometry is sound.

Warnings do not stop the check. The validity ranges of 6.4.3.1 (β, γ, θ) are reported as warnings
because the standard's own rule there is to compute with the parameters clamped to the range and
keep the lower capacity.

## Scope

Chapter **6.4** — tubular joints. Chapter 6.3 (tubular members) is not included, so no member
checks are performed.

## If something goes wrong

`norsok_app.log`, written next to the `.exe`, records the normal flow as well as errors: which
file was opened, which service version, how many load effects, how many checks passed or were
skipped and why.

| Message | Meaning |
|---|---|
| No usable IDEA StatiCa installation found | Nothing from 26.0 up was located. The message lists where it looked; use `IDEA_CONNECTION_REST_EXE` if your installation is elsewhere. |
| Service did not come up within 30 s | The service was found but failed to start — usually a licence or .NET runtime problem on its side. |
| Blank window | The WebView2 runtime is missing (see above). |
| Opening a file fails | The project was saved by a newer version of IDEA StatiCa than the service being used. Open and re-save it in that version, or install it. |
