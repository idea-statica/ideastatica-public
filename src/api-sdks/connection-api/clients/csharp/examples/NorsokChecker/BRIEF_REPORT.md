# Brief — the NORSOK §6.4 report

> **Purpose:** the exported PDF must stand on its own for a structural engineer who has never seen
> this tool and is not told how the document was produced.

That sentence is the acceptance criterion, and it is not decoration: it came out of two rounds of
review by an engineer who had the standard, the PDF and nothing else. Every requirement below
failed it at least once.

## Requirements

1. **No verdict without its caveat, in the row that is scanned.** A qualifier that reaches only the
   detail card sixty pages away has not been reported. Applies to geometry outside the §6.4.3.1
   validity ranges (`QUALIFIED`), and to every future caveat of the same shape.
2. **State WHICH, never just THAT.** A count ("2 conditions"), a bare dash, or "outside range" sends
   the reader hunting. Name the parameter and its value: `M1: θ = 20.0°, outside 30–90°`.
3. **One number, one unit.** `Total Checks: 55` added 30 checks to 25 unmet conditions. Counters are
   per unit, and a connection is counted in connections.
4. **A printed number must be traceable to the model.** The checks run on forces resolved into a
   plane; printing only the result made the whole document unverifiable against the IDEA StatiCa
   model. Both frames appear, side by side, at the state that governs each brace.
5. **Say what a printed quantity is FOR.** A number in a column of resistance inputs will be read as
   a resistance input. `off-plane` is a coplanarity check, not a projection parameter, and the
   document has to say so where it prints it.
6. **Tool thresholds are declared as ours.** The 2° coplanarity tolerance, >5 mm out-of-plane
   eccentricity and >15° off-plane are settings, not §6.4 requirements, and they print beside real
   clause references.
7. **The document does not overclaim.** Not "Compliance Report" for a report in which connections
   routinely go unassessed; not "COMPLIANT" when a check ran on extrapolated formulas; not
   "excluded by §6.4" for actions the clause simply does not cover.
8. **Nothing in the report grows with the number of load effects.** A model may hold arbitrarily
   many. Declare the set in counts; keep the per-brace rows, which are one per brace whatever the
   count.
9. **The report is an insert as well as a document.** It gets bound into someone else's calculation
   package, so pagination is configurable and `continuous` numbering prints no total.
10. **Every claim about the output is measured before it is written.** Three statements in our
    round-1 reply were wrong; `reference/verification_scripts/verify_report.py` exists so that
    cannot recur.

## Compromises

- **The page footer sits in the bottom margin.** The reviewer's round-1 spec forbade it (the margins
  are the user's); the rendering engine will only paginate a repeating element in a margin box, so
  the alternative was no page numbers on a 173-page document. Flagged to them, and they overruled
  their own rule in round 2. `footer.mode: off` is the escape hatch when a host document's own
  footer would collide.
- **PDF `/Subject` and `/Author` are absent, and `/Creator` names the browser engine.** Verified by
  compiling: `CoreWebView2PrintSettings` has no such properties. Setting them needs a
  post-processing pass over the finished PDF — the same pass `/Outlines` needs — and both are
  deferred together.
- **KaTeX stays in the equations.** A copied formula comes out in drawing order, which is a real
  defect of the output. Measured cost of removing it: the equations are not static strings, several
  are generated with substituted numbers, so it means rewriting the substitution generators with
  real risk to typesetting that works. Revisit if a reader names a workflow it blocks.
- **The §6.4.3.1 out-of-range rule stays two-variant** (actual vs all-infringed-clamped) rather than
  taking the lesser over every subset of the infringed parameters. Measured on the python reference:
  on physically possible geometry the subset reading is lower in ~42 % of out-of-range cases, median
  3 %. Kept because the clause names one set of imposed limits, not a family — but the report must
  not imply the two printed passes are an envelope, because they are not.
- **Unit formatting is not centralised.** 240 × `kN·m` against 60 × `kNm` in the reviewed sample.
  One formatter is the fix and it is not built.

## Assumptions

| Assumption | Invalidated when |
|---|---|
| One method chapter per connection, so a contents page has no hierarchy to map and is not printed | a second method (§6.3, CIDECT) produces rows — `ShouldRenderContents` then prints it, and §10.3's rules for it apply |
| §6.4 is the only chapter, so the overview is one row per connection | the same — the overview becomes a matrix; see *Decisions* below |
| The reader has the standard, or knows it | the audience widens to non-engineers; clause references would stop carrying their weight |
| The report is read on paper or as a PDF, in greyscale as often as in colour | never rely on colour alone; already a requirement, and the reason every status carries a word |
| A brace's own sub-plane frame is the right one for §6.4 (verified against the python reference) | the reference implementation changes, or the norm is read differently — then the identical CON12/CON13 force tables become a defect rather than a consequence |
| `counter(page)` in an `@page` margin box works in the pinned WebView2 (SDK 1.0.2903.40 = Chromium 131) | the pinned runtime moves below 131, or the export path changes away from WebView2 |

## Decisions recorded but NOT built

Round-3 §2 and §3.4–3.5, deliberately deferred: they describe structure that has nothing to display
while one method exists, so building it now means code no test can exercise on real output.

- **The overview becomes a matrix** — rows connections, columns methods, cells verdict plus
  utilisation. Readable to about four methods; beyond that transpose or split.
- **A cell needs three states, not two**: assessed / `N/A` the method does not cover this joint /
  `not run` the method was not enabled for this run. Today's single dash conflates them — the CON10
  problem in two dimensions.
- **A row where every method says `N/A` is the most important row and the least visible.** It needs
  a marker or a `covered by 0 of 3 methods` column, so the document states it rather than leaving
  the reader to notice a row of dashes.
- **Never aggregate a verdict across methods.** Different codes are not commensurable; collapsing
  `§6.4: N/A, §6.3: PASS 88 %, CIDECT: PASS 84 %` into "CON2: PASS" is `Total Checks: 55` again.
  Coverage may be stated; the verdict stays per method.
- **Verdicts and chapter numbers leave the contents** once it is printed at all: the overview says
  what the state is, the contents says where it is, and neither should answer the other's question.
  `CON1 / §6.4` is already a unique, stable key, whereas a `3.1` shifts whenever a connection is
  added.
- **A per-brace envelope** (min/max of each component across states) — optional, off by default.
- **The full state matrix leaves the PDF** as `<project>-load-effects.csv`. Second benefit: it is
  what `verify_report.py` needs in order to check that the governing state named in the report is
  the governing state in the data — a reconciliation that cannot be automated while the data never
  leaves the tool.
- **Left open deliberately:** if the matrix can carry a page reference inside each cell, it does both
  jobs and the contents is permanently redundant. Decide after the matrix exists, not before.

## Related

- Review correspondence: `01_Folders/NORSOK/review/` (outside the repo) — the round-1 and round-2
  specs, and our replies. Read its `README.md` first.
- Verification: `reference/verification_scripts/verify_report.py` — run it on an exported PDF before
  writing any claim about the output.
- The reference implementation: `reference/python_prototype/norsok/` — `n64.py` is the §6.4 engine
  the C# port is verified against, `extract.py` the force-reading and topology recipe.
