# Third-party components embedded in NorsokChecker

## KaTeX 0.16.11

Typesets the LaTeX formulas in the HTML report and in the §6.4 derivation window.

- Upstream: https://katex.org/ — https://github.com/KaTeX/KaTeX
- Licence: MIT (`LICENSE.katex.txt` beside this file). The same licence as this repository.
- Files: `katex.min.js`, `katex-auto-render.min.js`, `katex.min.css` — embedded resources, see
  `NorsokChecker.csproj`.

**Why it is vendored rather than fetched.** It used to be pulled from `cdn.jsdelivr.net`, which
meant that on a machine with no network the report showed the raw LaTeX source — `$$\dfrac{f_y
T^2}{\gamma_M \sin\theta}$$` — instead of the equation. A code-check report is a deliverable and is
often read where there is no internet, so the library has to travel with it.

**Modification.** `katex.min.css` is not the upstream file byte-for-byte: every `url(fonts/*.woff2)`
has been replaced with an inline `data:font/woff2;base64,...` URI, and the `ttf`/`woff` alternatives
for those faces were removed. Two reasons:

- the report is handed to WebView2 as a **string** (`NavigateToString`) and may be saved and moved,
  so there is no base URL for a relative `url(fonts/…)` to resolve against — external font files
  would fail even when they exist on disk;
- WebView2 is Chromium and always takes `woff2`, so shipping the other two formats would have
  tripled the size (1.2 MB → 296 kB) for faces nothing requests.

No JavaScript was altered. The rebuild is reproducible: unpack `katex-0.16.11.tgz` from npm and
apply the substitution above.

Total embedded size: 0.62 MB.

---

The python reference under `reference/python_prototype/lib/` keeps its own copies of MathJax and
three.js with their licences, on the same principle.
