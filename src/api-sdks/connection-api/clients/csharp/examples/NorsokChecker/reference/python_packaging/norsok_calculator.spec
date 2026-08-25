import os

# PyInstaller spec — single-file build of the NORSOK Joint Calculator.
#
# Build (from THIS folder):
#   py -m PyInstaller norsok_calculator.spec --noconfirm
# Result: dist/NorsokJointCalculator.exe — one file, hand that over as is.
# See README.md next to this file.
#
# One-file: 25 MB and nothing to explain to the recipient. Measured against the one-dir
# alternative (a 57 MB folder whose exe alone does not run): one-file costs about 5 s at launch,
# because it unpacks itself into a temp folder every time. That is nothing next to starting the
# REST service, and it removes the "do not take the exe out of the folder" trap.
#
# The usual one-file objection — "the log disappears into temp" — does not apply here: app.py
# keeps BUNDLE_DIR (sys._MEIPASS, the temp unpack, read-only bundled files) apart from DATA_DIR
# (sys.executable's folder, where we write), so norsok_app.log lands beside the exe. Verified.
#
# ui.html and lib/ are bundled at the TOP level of the bundle, because ui.html loads its
# libraries with relative paths ("lib/three.min.js"); app.py resolves them through BUNDLE_DIR.
#
# Not bundled: the IDEA StatiCa REST service. The app locates an installed one at run time
# (26.0 or newer — see service_exe in app.py).
#
# n63.py (chapter 6.3, tubular MEMBERS) is not wired into the app, but it is re-exported by
# norsok/__init__.py, so it comes along. Left that way deliberately: it is 12 kB, excluding it
# would break the package import, and 6.3 stays in the repo for whenever it does get wired in.

# The app itself lives one folder up; keep it that way so the prototype stays a plain,
# runnable python app and everything to do with packaging is confined here.
SRC = os.path.join(os.path.dirname(os.path.abspath(SPEC)), '..', 'python_prototype')

datas = [
    (os.path.join(SRC, 'ui.html'), '.'),
    (os.path.join(SRC, 'lib'), 'lib'),
]

a = Analysis(
    [os.path.join(SRC, 'app.py')],
    pathex=[SRC],          # so 'from norsok import extract' resolves
    binaries=[],
    datas=datas,
    hiddenimports=[],
    hookspath=[],
    runtime_hooks=[],
    # numpy is needed (extract.py); tkinter is not — pywebview uses the Edge WebView2 runtime
    # on Windows, and excluding tkinter keeps the bundle from pulling in the whole Tcl/Tk tree.
    excludes=['tkinter', 'matplotlib', 'pytest'],
    noarchive=False,
)
pyz = PYZ(a.pure)

# Single file: EXE takes the binaries and datas directly and there is no COLLECT — that absence
# is what makes it one-file in PyInstaller 6 (there is no 'onefile' argument).
exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.datas,
    [],
    name='NorsokJointCalculator',
    debug=False,
    strip=False,
    upx=False,
    console=False,          # windowed: the UI is the window, diagnostics go to norsok_app.log
)
