# CBFEM plate / weld / bolt checks — mothballed 2026-09-01

This folder holds the CBFEM chapter: von-Mises checks on plates, equivalent-stress checks on welds
and tension/shear interaction on bolts, all with NORSOK Table 6-1 material factors
(γ_M0 = 1.15, γ_M2 = 1.30) rather than the EC3 defaults. Nothing outside this folder calls it.

## Why it was shelved

**It duplicated IDEA StatiCa Connection's own function.** Connection already performs these checks
on the same model, and an engineer working there sees them in the context they were designed in.
Re-running a calculation in this app to obtain a weaker version of that is not worth the code.

**It was never reviewed or authorised.** The chapter was written to see what was possible, not to a
specification anyone approved. That is the reason it is kept rather than deleted — the norm reading
inside it (which factor applies where, why welds take γ_M2, why a plate with no reported stress is
"not assessed" rather than 0 %) cost real work and may be worth reading again — and equally the
reason it is not simply switched back on.

## If the chapter comes back

**Assume it will be rebuilt, not revived.** The app's structure changed after this was shelved:
chapters now go through `Services/Chapters/IChapter`, the run loop iterates a registry rather than
a hardcoded list, and the verdict roll-up moved into `Services/CheckWorkflow`. The code here
predates all of that and will not plug in as it stands.

What it needs to run at all:
- A CBFEM calculation. §6.4 needs none — it works from load effects and geometry — so with this
  chapter gone the app no longer calls `CalculateAsync` at all. That call, and the raw-results
  fetch, would have to come back.
- `RawResultsParser` (in this folder) to turn the raw JSON into plates, welds and bolts.
- The material factors written into the project before the run — still done, by
  `Services/ProjectSettingsService`, because §6.4 needs them too.

Removed along with the chapter, and worth knowing about:
- **`ActivateAllLoadEffectsAsync`** switched every load effect on before calculating, because the
  26.0 client's `CalculateAsync` takes no load-effect selector. Only the CBFEM side needed it; §6.4
  filters its own set. If CBFEM returns, so does that problem.
- **Member thickness refinement.** `t` and `f_y` in the members grid were refined from the modelled
  plate names in the raw results. §6.4 does not depend on it (D/T come from the IOM facet ring, per
  connection and measured — see `Services/Norsok64/TubeFromIom.cs`), so it went with the chapter.
- **The IDEA StatiCa PDF.** The export used to write a second file via the API. Removed for the same
  reason as the chapter: the report belongs where the model is worked on.

## Files

| file | what it holds |
|---|---|
| `CbfemChecks.cs` | the three evaluators — plates (von Mises vs f_y/γ_M0), welds (equivalent stress vs the engine's resistance, or f_u/(β_w·γ_M2)), bolts (tension/shear interaction) |
| `RawResultsParser.cs` | raw CBFEM JSON → plates / welds / bolts, with stresses, resistances and materials |

## If the calculate path ever returns

**Pair the results to the connections by `Id`, never by position in the array.** The run sends only
the ticked connections, so the response is shorter than the connection list and its indices do not
line up: pairing by index awards CON2's utilisation to CON3 whenever anything is unticked, which
looks plausible and is wrong. This was covered by a test that outlived the code it described
(`ConnectionSelectionTests.ResultsArePairedByIdNotByPosition`); the test is gone, the rule is not.
