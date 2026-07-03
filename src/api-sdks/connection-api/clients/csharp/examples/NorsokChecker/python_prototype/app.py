# -*- coding: utf-8 -*-
"""NORSOK Joint Calculator — standalone desktop app (pywebview).

Originally branched 2026-06-28 from an earlier read-only joint viewer (since removed —
this is now the single app). It grows the 6.4 check pipeline on top of that geometry:
force->plane transform, K/Y/X classification, per-gap force-balance fractions, then the
actual NORSOK 6.4 verification.

No HTTP server, no open port: the HTML/JS UI runs in a native window and calls
Python methods directly via window.pywebview.api.*.

Lifecycle:
  - on demand, ensures the IDEA Connection REST service is running:
      * if it already answers -> use it, do NOT shut it down (we didn't start it)
      * if not -> launch IdeaStatiCa.ConnectionRestApi.exe, wait, remember we own it
  - native file dialog to pick an .ideaCon
  - extract geometry (extract.py) and hand JSON to the UI
  - on window close, shut the service down ONLY if we started it
"""
import os, sys, time, subprocess, json, logging, traceback
from logging.handlers import RotatingFileHandler
import requests
import webview
from norsok import extract   # data/API + NORSOK calc layer (extract.py + n64.py/n63.py live in norsok/)

EXE = r"C:\Program Files\IDEA StatiCa\StatiCa 26.0\IdeaStatiCa.ConnectionRestApi.exe"
BASE = "http://localhost:5000/api/4"
VERSION_EP = f"{BASE}/clients/idea-service-version"
HERE = os.path.dirname(os.path.abspath(__file__))

# --- logging ---------------------------------------------------------------
# One rotating log (norsok_app.log) next to the app. It records not just crashes but the normal
# flow — which file was opened, how many load effects, how many 6.4 checks passed/skipped — so a UI
# error like "division by zero" is never a dead end: the log says which brace/LE and which line.
# Rotation caps it at ~1 MB x 3 files so it never grows unbounded during long sessions.
LOG_PATH = os.path.join(HERE, "norsok_app.log")
_fh = RotatingFileHandler(LOG_PATH, maxBytes=1_000_000, backupCount=3, encoding="utf-8")
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)-7s %(message)s",
    handlers=[_fh, logging.StreamHandler()],
)
log = logging.getLogger("norsok")
log.info("=" * 60)
log.info("NORSOK Joint Calculator starting — log at %s", LOG_PATH)


def service_alive(timeout=2):
    try:
        r = requests.get(VERSION_EP, timeout=timeout)
        return r.status_code == 200
    except Exception:
        return False


class Api:
    def __init__(self):
        self._owns_service = False   # True only if WE launched the exe
        self._proc = None
        self._session = None
        self._pid = None
        self._source = None

    # ---- service lifecycle ----
    def ensure_service(self):
        """Return dict {ok, started_by_us, version, msg}."""
        if service_alive():
            ver = requests.get(VERSION_EP, timeout=4).text.strip().strip('"')
            return {"ok": True, "started_by_us": False, "version": ver,
                    "msg": f"Service už běží (v{ver}) — nepřebíráme ji."}
        if not os.path.exists(EXE):
            return {"ok": False, "msg": f"REST service exe nenalezeno:\n{EXE}"}
        try:
            # no shell, fixed path, no user input -> no injection surface
            self._proc = subprocess.Popen([EXE],
                                          stdout=subprocess.DEVNULL,
                                          stderr=subprocess.DEVNULL)
        except Exception as e:
            return {"ok": False, "msg": f"Nepodařilo se spustit service: {e}"}
        # wait up to ~30 s for it to come up
        for _ in range(60):
            if service_alive(timeout=1):
                self._owns_service = True
                ver = requests.get(VERSION_EP, timeout=4).text.strip().strip('"')
                return {"ok": True, "started_by_us": True, "version": ver,
                        "msg": f"Service spuštěna námi (v{ver}) — po zavření ji vypneme."}
            time.sleep(0.5)
        return {"ok": False, "msg": "Service nenaběhla do 30 s."}

    def shutdown_service(self):
        """Close project; kill the exe only if we started it."""
        if self._pid and self._session is not None:
            extract.close_project(self._session, self._pid)
            self._pid = None
        if self._owns_service and self._proc and self._proc.poll() is None:
            try:
                self._proc.terminate()
                try:
                    self._proc.wait(timeout=5)
                except subprocess.TimeoutExpired:
                    self._proc.kill()
            except Exception:
                pass
            self._owns_service = False

    # ---- UI-callable ----
    def pick_file(self):
        """Open a native open-file dialog, return the chosen path (or '')."""
        win = webview.windows[0]
        res = win.create_file_dialog(
            webview.OPEN_DIALOG, allow_multiple=False,
            file_types=("IDEA Connection (*.ideaCon)", "All files (*.*)"))
        if not res:
            return ""
        return res[0]

    def open_file(self, path):
        """Ensure service, open the project, return {connections:[{id,name}], service, pid}.
        Keeps the project open so connections can be built on demand."""
        log.info("open_file: %s", path)
        if not path or not os.path.exists(path):
            log.warning("open_file: file does not exist: %s", path)
            return {"error": "Soubor neexistuje."}
        st = self.ensure_service()
        if not st["ok"]:
            log.error("open_file: service not available: %s", st["msg"])
            return {"error": st["msg"]}
        try:
            if self._session is None:
                self._session = requests.Session()
                extract.connect(self._session)
            # close any previously open project
            if self._pid:
                extract.close_project(self._session, self._pid)
            pid, conns = extract.open_and_list(self._session, path)
            self._pid = pid
            self._source = os.path.basename(path)
            log.info("open_file OK: %s -> pid=%s, %d connection(s): %s",
                     self._source, pid, len(conns), [c.get("name") for c in conns])
            return {
                "service": st["msg"],
                "source": self._source,
                "connections": [{"id": c["id"], "name": c.get("name")} for c in conns],
            }
        except Exception as e:
            log.exception("open_file failed for %s", path)
            return {"error": f"Chyba otevření: {e}"}

    def build_connection(self, conn_id, oop_tol_mm=5.0, plane_tol_deg=2.0, coplanar_tol_deg=15.0,
                         kyx_gate_pct=0.0):
        """Build viewer payload for one connection id (project must be open).
        oop_tol_mm = out-of-plane eccentricity tol; plane_tol_deg = RANSAC FIT tol (strict, builds plane);
        coplanar_tol_deg = EVALUATION tol (member beyond it -> multiplanar);
        kyx_gate_pct = K/Y/X "balanced within X %" gate in PERCENT (0 = honest breakdown)."""
        if not self._pid:
            log.warning("build_connection called with no project open")
            return {"error": "Projekt není otevřen."}
        log.info("build_connection: conn_id=%s (oop=%.1fmm plane=%.1f° coplanar=%.1f° gate=%.1f%%)",
                 conn_id, oop_tol_mm, plane_tol_deg, coplanar_tol_deg, kyx_gate_pct)
        try:
            data = extract.build_for(self._session, self._pid, int(conn_id),
                                     oop_tol_mm=float(oop_tol_mm),
                                     plane_tol_deg=float(plane_tol_deg),
                                     coplanar_tol_deg=float(coplanar_tol_deg),
                                     kyx_gate=float(kyx_gate_pct) / 100.0)
            data["source"] = self._source
            # summarise the outcome so the log tells the story even when nothing crashed
            jc = data.get("joint_checks") or []
            nreal = sum(1 for le in jc for b in le.get("braces", []) if not b.get("skipped"))
            nskip = sum(1 for le in jc for b in le.get("braces", []) if b.get("skipped"))
            nfail = sum(1 for le in jc for b in le.get("braces", [])
                        if not b.get("skipped") and not b.get("passed"))
            log.info("build_connection OK: conn_id=%s, %d LE, 6.4 checks: %d real (%d FAIL) + %d skipped",
                     conn_id, len(jc), nreal, nfail, nskip)
            return data
        except Exception as e:
            log.exception("build_connection failed for conn_id=%s", conn_id)
            # surface the exception TYPE too — a bare "division by zero" is otherwise opaque;
            # the full traceback (which brace/LE, which line) is in norsok_app.log.
            return {"error": f"Chyba extrakce: {type(e).__name__}: {e}\n(detail v {LOG_PATH})"}

    def ui_log(self, level, message):
        """Log a message coming from the JS/UI layer into the same file, so front-end errors
        (a thrown render, a bad state) land next to the Python flow instead of only in the
        webview devtools console. Called from ui.html via window.pywebview.api.ui_log(...)."""
        try:
            lvl = {"error": logging.ERROR, "warn": logging.WARNING,
                   "warning": logging.WARNING, "info": logging.INFO,
                   "debug": logging.DEBUG}.get(str(level).lower(), logging.INFO)
            log.log(lvl, "[UI] %s", message)
        except Exception:
            pass
        return True


def main():
    api = Api()
    html_path = os.path.join(HERE, "ui.html")
    window = webview.create_window("NORSOK Joint Calculator", html_path,
                                   js_api=api, width=1280, height=820,
                                   min_size=(900, 600))

    def on_closed():
        api.shutdown_service()

    window.events.closed += on_closed
    webview.start()


if __name__ == "__main__":
    main()
