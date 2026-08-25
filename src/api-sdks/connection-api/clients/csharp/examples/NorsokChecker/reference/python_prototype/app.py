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
import os, sys, time, socket, subprocess, json, logging, traceback
from logging.handlers import RotatingFileHandler
import requests
import webview
from norsok import extract   # data/API + NORSOK calc layer (extract.py + n64.py/n63.py live in norsok/)

EXE = r"C:\Program Files\IDEA StatiCa\StatiCa 26.0\IdeaStatiCa.ConnectionRestApi.exe"
# Port of a service we did NOT start: the service's own default. Only used to detect an
# already-running instance; a service we launch ourselves gets a free port (see start_service).
DEFAULT_PORT = 5000
HERE = os.path.dirname(os.path.abspath(__file__))


def api_base(port):
    return f"http://127.0.0.1:{port}/api/4"


def version_ep(port):
    return f"{api_base(port)}/clients/idea-service-version"


def free_port():
    """Let the OS pick a free port (bind to 0, read it back, release). Same approach as the
    C# ConnectionApiServiceRunner — passing it to the service with -port= makes a port
    collision impossible, so the app never fails because something else holds 5000."""
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        s.bind(("127.0.0.1", 0))
        return s.getsockname()[1]
    finally:
        s.close()

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


def service_alive(port, timeout=2):
    try:
        r = requests.get(version_ep(port), timeout=timeout)
        return r.status_code == 200
    except Exception:
        return False


def service_version(port, timeout=4):
    return requests.get(version_ep(port), timeout=timeout).text.strip().strip('"')


class Api:
    def __init__(self):
        self._owns_service = False   # True only if WE launched the exe
        self._port = DEFAULT_PORT
        self._proc = None
        self._session = None
        self._pid = None
        self._source = None

    # ---- service lifecycle ----
    def ensure_service(self):
        """Return dict {ok, started_by_us, version, msg}.

        An instance already listening on the service's default port is reused as-is (we never
        own it). Otherwise we launch our own on an OS-assigned free port, so a busy 5000 —
        another IDEA app, an unrelated dev server — can no longer stop this app from starting."""
        if service_alive(DEFAULT_PORT):
            self._port = DEFAULT_PORT
            ver = service_version(DEFAULT_PORT)
            return {"ok": True, "started_by_us": False, "version": ver,
                    "msg": f"Service already running on port {DEFAULT_PORT} (v{ver}) — not taking it over."}
        if not os.path.exists(EXE):
            return {"ok": False, "msg": f"REST service exe not found:\n{EXE}"}
        port = free_port()
        try:
            # no shell, fixed path, no user input -> no injection surface
            # -port= is the service's own switch (the standard ASP.NET --urls is ignored by it)
            self._proc = subprocess.Popen([EXE, f"-port={port}"],
                                          stdout=subprocess.DEVNULL,
                                          stderr=subprocess.DEVNULL)
        except Exception as e:
            return {"ok": False, "msg": f"Failed to start the service: {e}"}
        # wait up to ~30 s for it to come up
        for _ in range(60):
            if service_alive(port, timeout=1):
                self._owns_service = True
                self._port = port
                ver = service_version(port)
                return {"ok": True, "started_by_us": True, "version": ver,
                        "msg": f"Service started by us on port {port} (v{ver}) — it will be shut down on exit."}
            time.sleep(0.5)
        return {"ok": False, "msg": f"Service did not come up on port {port} within 30 s."}

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
            return {"error": "File does not exist."}
        st = self.ensure_service()
        if not st["ok"]:
            log.error("open_file: service not available: %s", st["msg"])
            return {"error": st["msg"]}
        # the port is only known once the service is up — point extract.py at the same one
        extract.set_base(api_base(self._port))
        log.info("service at %s (owned by us: %s)", api_base(self._port), self._owns_service)
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
            # drop the session so the next attempt gets a fresh ClientId — otherwise a single
            # failed request (e.g. a transient 500 from the service) leaves every subsequent
            # call 401ing on the now-unrecognized ClientId until the app itself is restarted.
            self._session = None
            self._pid = None
            return {"error": f"Failed to open: {e}"}

    def build_connection(self, conn_id, oop_tol_mm=5.0, plane_tol_deg=2.0, coplanar_tol_deg=15.0,
                         kyx_gate_pct=0.0):
        """Build viewer payload for one connection id (project must be open).
        oop_tol_mm = out-of-plane eccentricity tol; plane_tol_deg = RANSAC FIT tol (strict, builds plane);
        coplanar_tol_deg = EVALUATION tol (member beyond it -> multiplanar);
        kyx_gate_pct = K/Y/X "balanced within X %" gate in PERCENT (0 = honest breakdown)."""
        if not self._pid:
            log.warning("build_connection called with no project open")
            return {"error": "No project is open."}
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
            return {"error": f"Extraction failed: {type(e).__name__}: {e}\n(details in {LOG_PATH})"}

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
