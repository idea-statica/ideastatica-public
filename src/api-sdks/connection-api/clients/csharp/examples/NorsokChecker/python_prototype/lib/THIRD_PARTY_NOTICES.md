# Third-party notices

This app vendors the following third-party JavaScript libraries under `lib/`, used
by `ui.html` for math rendering and 3D visualization. No CDN calls are made at runtime.

## MathJax

- File: `mathjax-tex-svg.js`
- Component: `tex-svg` (TeX input -> SVG output combined component)
- Version: **3.2.2**, confirmed by SHA-256 match against the official build
  `https://cdnjs.cloudflare.com/ajax/libs/mathjax/3.2.2/es5/tex-svg.js`
  (sha256 `d4295dc33744836935c1399feece5159577b34c5c8ffb9f1c6324cd82e03a882`, 2108580 bytes)
- License: Apache License 2.0 — see `LICENSE.mathjax.txt`
- Source: https://github.com/mathjax/MathJax-src
- Known CVEs: CVE-2018-1999024 (XSS, fixed in 2.7.4) and CVE-2023-39663 (ReDoS, disputed
  by vendor) both apply only to the MathJax 2.x line. Neither affects 3.2.2.
- Note: 3.2.2 is the last release of the MathJax 3.x line; current stable is 4.1.2
  (as of 2026-07). No known CVE against 3.2.2 itself, but it is behind current.

## Three.js

- File: `three.min.js`
- Version: **r160** (npm `0.160.0`), confirmed by the `@license` header in the file
  and by the `THREE.REVISION` value (`"160"`) embedded in the minified source
- License: MIT — see `LICENSE.three.js.txt`
- Source: https://github.com/mrdoob/three.js
