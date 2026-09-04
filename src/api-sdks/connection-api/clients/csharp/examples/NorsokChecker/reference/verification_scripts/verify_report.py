#!/usr/bin/env python3
"""Measure an exported NORSOK report PDF, before making any claim about it.

    python verify_report.py <report.pdf>

Why this exists: three measurements in our round-1 reply to the report reviewer were wrong, and
the reviewer found them. Two were quoted from memory and one was an illustrative table whose
numbers appear nowhere in the file. Their round-2 §7 asked for exactly this script, and it is the
cheapest possible answer -- every false statement we sent was a one-line check here.

It also settles the class of dispute that produced the OTHER half of round 2: two PDF text
extractors disagree about this document, and each of us trusted one of them. `pdftotext` reports
thousands of U+200B and no NBSP; pypdf reports the opposite. Neither is lying -- KaTeX draws a
spacing strut, and the two libraries name it differently. So where the extractors disagree, this
script decides at FONT level (does any ToUnicode CMap map the character at all?), which does not
depend on either.

Exit code 0 when every check passes, 1 otherwise. Nothing here is a CI gate: it is a tool to run
before writing a sentence about the output.
"""
from __future__ import annotations

import re
import subprocess
import sys
from pathlib import Path

# The report is full of typography -- section signs, en dashes, the disclosure triangles this very
# script hunts for -- and a Windows console defaults to cp1252, so printing a finding raised
# UnicodeEncodeError and the run died half way through its own checks. Reconfiguring the streams
# here means the script no longer has to be invoked with PYTHONUTF8=1 to survive its own output.
for _stream in (sys.stdout, sys.stderr):
    try:
        _stream.reconfigure(encoding="utf-8", errors="replace")
    except Exception:
        pass

try:
    import pypdf
except ImportError:
    sys.exit("pypdf is required:  python -m pip install --user pypdf")


# ── the nine text defects from round 1, all of which must stay at zero ────────────────────────
TEXT_DEFECTS = {
    "§6.4 §6.4 (doubled section)": "§6.4 §6.4",
    "(Eq. -) placeholder": "(Eq. -)",
    "(Eq. 6.4.3) on a clause": "(Eq. 6.4.3)",
    "US 'Utilization'": "Utilization",
    "ASCII <=": "<=",
    "ASCII >=": ">=",
    "raw 'beta' in a condition": "0.2<=beta",
    "disclosure triangle ▸": "▸",
    "disclosure triangle ▾": "▾",
}

# Defects that need a REGEX, because the plain substring occurs in legitimate prose. Learned the
# hard way in this very script: a bare "deg" search reported 16 hits, all of them the word
# "degenerate" in "parallel to chord (degenerate)". A needle that matches something innocent is a
# needle that cannot go missing -- exactly the failure mode this script exists to catch.
TEXT_DEFECT_PATTERNS = {
    "ASCII 'deg' after a number": r"\d\s*deg\b",
}

# Derivation headings that were left at the foot of a page in the shipped report. The count is
# what matters, not the list -- any heading ending a page is the defect.
HEADINGS = (
    "Members — geometry at the joint",
    "Utilisation — eq (6.57)",
    "Weighted axial resistance",
    "Joint plane and force transformation",
    "Basic assumptions — validity ranges",
    "Out-of-range rule",
    "Chord stress derivation",
    "Geometry & material",
    "Applied forces",
    "Axial resistance",
)


class Report:
    def __init__(self, path: Path):
        self.path = path
        self.reader = pypdf.PdfReader(str(path))
        self.pages = [p.extract_text() for p in self.reader.pages]
        self.text = "".join(self.pages)
        self._layout: list[str] | None = None

    # ── pdftotext, when available: the reviewer's own tool, so the numbers are comparable ──
    @property
    def layout_pages(self) -> list[str] | None:
        """Pages as `pdftotext -layout` sees them, or None when the tool is absent."""
        if self._layout is not None:
            return self._layout or None
        try:
            out = subprocess.run(
                ["pdftotext", "-layout", str(self.path), "-"],
                capture_output=True, check=True,
            ).stdout.decode("latin-1")          # pdftotext writes latin-1 here, not UTF-8
        except (FileNotFoundError, subprocess.CalledProcessError):
            self._layout = []
            return None
        self._layout = out.split("\f")
        return self._layout

    def fonts_mapping(self, codepoint: int) -> list[str]:
        """Fonts whose ToUnicode CMap maps this codepoint -- the extractor-independent answer."""
        needle = f"{codepoint:04x}"
        hits, seen = [], set()
        for page in self.reader.pages:
            for font in page.get("/Resources", {}).get("/Font", {}).values():
                obj = font.get_object()
                name = str(obj.get("/BaseFont"))
                if name in seen or "/ToUnicode" not in obj:
                    continue
                seen.add(name)
                cmap = obj["/ToUnicode"].get_object().get_data().decode("latin-1").lower()
                if needle in cmap:
                    hits.append(name)
        return hits


class Checks:
    def __init__(self):
        self.rows: list[tuple[bool, str, str]] = []

    def add(self, ok: bool, name: str, detail: str = ""):
        self.rows.append((ok, name, detail))

    def note(self, name: str, detail: str):
        self.rows.append((None, name, detail))

    def report(self) -> int:
        width = max(len(n) for _, n, _ in self.rows)
        failed = 0
        for ok, name, detail in self.rows:
            mark = "  --" if ok is None else (" OK " if ok else "FAIL")
            print(f"[{mark}] {name.ljust(width)}  {detail}")
            failed += ok is False
        print()
        print(f"{len(self.rows)} checks, {failed} failed")
        return 1 if failed else 0


def main(argv: list[str]) -> int:
    if len(argv) != 2:
        print(__doc__)
        return 2
    path = Path(argv[1])
    if not path.exists():
        return print(f"no such file: {path}") or 2

    r = Report(path)
    c = Checks()

    # ── page geometry ────────────────────────────────────────────────────────────────────────
    box = r.reader.pages[0].mediabox
    w_mm, h_mm = float(box.width) * 25.4 / 72, float(box.height) * 25.4 / 72
    a4 = abs(w_mm - 210) < 2 and abs(h_mm - 297) < 2
    c.add(a4, "A4 page size", f"{w_mm:.1f} x {h_mm:.1f} mm"
          + ("" if a4 else "  <- US Letter is WebView2's default; PageSetup was not applied"))
    sizes = {(round(float(p.mediabox.width)), round(float(p.mediabox.height)))
             for p in r.reader.pages}
    c.add(len(sizes) == 1, "one page size throughout", f"{len(sizes)} distinct")
    c.note("page count", f"{len(r.reader.pages)}  (173 in the round-2 sample)")

    # ── the nine text defects ────────────────────────────────────────────────────────────────
    for name, needle in TEXT_DEFECTS.items():
        n = r.text.count(needle)
        c.add(n == 0, f"no {name}", "" if n == 0 else f"{n} occurrence(s)")
    for name, pattern in TEXT_DEFECT_PATTERNS.items():
        hits = re.findall(pattern, r.text)
        c.add(not hits, f"no {name}",
              "" if not hits else f"{len(hits)}: {', '.join(sorted(set(hits))[:5])}")

    # ── orphaned headings: the round-2 §5.2 defect, measured at 25 ────────────────────────────
    pages = r.layout_pages
    if pages is None:
        c.note("orphaned headings", "pdftotext not on PATH -- cannot measure (install poppler)")
    else:
        orphans: list[tuple[int, str]] = []
        for i, page in enumerate(pages[:len(r.reader.pages)], 1):
            lines = [ln.strip() for ln in page.splitlines() if ln.strip()]
            lines = [ln for ln in lines if "NORSOK N-004" not in ln]   # drop the footer by TEXT
            if not lines:
                continue
            last = lines[-1]
            for h in HEADINGS:
                # -layout renders the em dash as '--' and drops '&'; compare on a loose form
                loose = h.replace("—", "--").replace("&", "")
                if last.startswith(loose) or last.startswith(h):
                    orphans.append((i, last[:60]))
                    break
        c.add(not orphans, "no heading left at a page foot",
              "" if not orphans else f"{len(orphans)}: "
              + ", ".join(f"p{p}" for p, _ in orphans[:12]))

        # ── page fill: the other half of the same cause ──────────────────────────────────────
        #
        # Measured against the FULLEST page in the document, not against each page's own line
        # count. `pdftotext -layout` emits no trailing blank lines, so a three-quarters-empty page
        # comes back with few lines and its last inked line near the end of them: page 19 of the
        # sample -- the reviewer's clearest case of wasted space -- has 18 lines with the last ink
        # on line 15, which as a self-relative ratio reads 83 % full. Dividing by the tallest page
        # (66 lines) instead gives 24 %, which is what the eye sees.
        # A PAGE CARRYING A FIGURE IS NOT AN EMPTY PAGE.
        #
        # The line count above is blind to anything that is not text, so a chapter-opening page
        # whose upper half is a joint figure reads as nearly empty and this check reported a defect
        # on the title page. Measured on the 227-page export: p1 66 %, p5/p39/p66 38/38/22 % -- all
        # four are figure pages and none is wasted space. The one real finding in that run (p35, a
        # single verdict line at 2 %) has no image at all, which is exactly the discrimination the
        # image area restores.
        #
        # A page's fill is its text fill PLUS the height its images take, capped at 1. Not the
        # greater of the two: a figure page carries both, one above the other, and taking the max
        # left p5 at 55 % (image alone) when text and picture together fill it -- measured, the
        # first version of this fix changed nothing at all because of that. Images are read per
        # page from the PDF resources, so a page with none behaves exactly as before.
        def image_fraction(page_index: int) -> float:
            try:
                pg = r.reader.pages[page_index]
                box = pg.mediabox
                page_h = float(box.height) or 1.0
                res = pg.get("/Resources") or {}
                xobjs = res.get("/XObject")
                if not xobjs:
                    return 0.0
                xobjs = xobjs.get_object()
                # A drawn image's height on the page is in the content stream's CTM, which is more
                # than this needs; the image's own pixel height against the page height is a good
                # enough proxy for "does this page hold a picture worth a third of it".
                total = 0.0
                for name in xobjs:
                    ob = xobjs[name].get_object()
                    if ob.get("/Subtype") == "/Image":
                        h = float(ob.get("/Height", 0) or 0)
                        # 96 dpi is what the generator draws at; converting to points keeps the
                        # comparison with page height honest.
                        total += h * 72.0 / 96.0
                return min(total / page_h, 1.0)
            except Exception:
                return 0.0

        last_inked = []
        for i, page in enumerate(pages[:len(r.reader.pages)], 1):
            lines = page.split("\n")
            inked = [j for j, ln in enumerate(lines)
                     if ln.strip() and "NORSOK N-004" not in ln]
            last_inked.append((i, (max(inked) + 1) if inked else 0))
        tallest = max(n for _, n in last_inked) or 1
        fills = [(i, min(n / tallest + image_fraction(i - 1), 1.0)) for i, n in last_inked]
        thin = sorted((p for p, f in fills if f < 0.65), key=lambda p: p)
        median = sorted(f for _, f in fills)[len(fills) // 2]
        c.add(len(thin) <= len(fills) * 0.1, "pages are not left mostly blank",
              f"{len(thin)} under 65 % fill, median {median * 100:.0f} %"
              + ("" if not thin else "  pages: " + ", ".join(map(str, thin[:12]))))

    # ── invisible characters: settled at FONT level, not by an extractor ─────────────────────
    for cp, label in ((0x200B, "ZWSP U+200B"), (0x200C, "ZWNJ"), (0x200D, "ZWJ"),
                      (0xFEFF, "BOM U+FEFF")):
        fonts = r.fonts_mapping(cp)
        c.add(not fonts, f"no {label} in any font map",
              "" if not fonts else f"mapped by {', '.join(fonts)}")
    nbsp = r.text.count(" ")
    c.note("U+00A0 (KaTeX spacing strut)",
           f"{nbsp}  -- 295 in the sample; harmless as a separator, but it is what the reviewer's "
           "tool reported as U+200B")

    # ── units: through BOTH extractors, because one strut prints two ways ───────────────────
    pypdf_mid, pypdf_run = r.text.count("kN·m"), r.text.count("kNm")
    detail = f"pypdf: {pypdf_mid} x 'kN.m', {pypdf_run} x 'kNm'"
    if pages is not None:
        joined = "".join(pages)
        detail += f" | pdftotext: {joined.count('kN·m')} x 'kN.m', {joined.count('kN m')} x 'kN m'"
    c.note("unit variants", detail + "   (one formatter is still deferred)")

    # ── document properties: it must not announce itself as a browser artefact ───────────────
    meta = dict(r.reader.metadata or {})
    creator = str(meta.get("/Creator", ""))
    c.add("Mozilla" not in creator and "Chrome" not in creator,
          "/Creator is not a browser UA", creator[:70] or "(absent)")
    c.add(bool(meta.get("/Title")), "/Title set", str(meta.get("/Title", "(absent)"))[:60])
    c.add("Compliance Report" not in str(meta.get("/Title", "")),
          "/Title does not overclaim compliance",
          "9 of 15 connections unassessed in the sample")
    for field in ("/Subject", "/Author"):
        c.add(bool(meta.get(field)), f"{field} set", str(meta.get(field, "(absent)"))[:50])
    raw = path.read_bytes()
    c.add(b"/Outlines" in raw, "PDF bookmarks (/Outlines)", "deferred -- expected to fail")

    # ── the round-2 §4.1 defect: the validity caveat must reach the front matter ─────────────
    front = "".join(r.pages[:3])
    total = len(re.findall(r"validity", r.text, re.I))
    in_front = len(re.findall(r"validity|QUALIFIED", front, re.I))
    c.add(total == 0 or in_front > 0, "validity caveat reaches pages 1-3",
          f"{in_front} on pages 1-3, {total} in the document")

    return c.report()


if __name__ == "__main__":
    sys.exit(main(sys.argv))
