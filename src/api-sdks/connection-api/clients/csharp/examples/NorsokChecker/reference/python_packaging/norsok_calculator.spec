import os

# PyInstaller spec — one-dir build of the NORSOK Joint Calculator.
#
# Build (from THIS folder):
#   py -m PyInstaller norsok_calculator.spec --noconfirm
# Result: dist/NorsokJointCalculator/NorsokJointCalculator.exe  (ship the whole folder, zipped)
# See README.md next to this file.
#
# One-dir, not one-file, on purpose: it starts immediately (no unpacking on every launch), the
# log lands next to the exe where the user can find it, and a failure is diagnosable because the
# parts are visible. The trade-off is that the customer gets a folder, not a single file.
#
# ui.html and lib/ are bundled at the TOP level of the bundle, because ui.html loads its
# libraries with relative paths ("lib/three.min.js"); app.py resolves it through BUNDLE_DIR
# (sys._MEIPASS when frozen), while the log goes to DATA_DIR (next to the exe).
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

exe = EXE(
    pyz,
    a.scripts,
    [],
    exclude_binaries=True,
    name='NorsokJointCalculator',
    debug=False,
    strip=False,
    upx=False,
    console=False,          # windowed: the UI is the window, diagnostics go to norsok_app.log
)
coll = COLLECT(
    exe,
    a.binaries,
    a.datas,
    strip=False,
    upx=False,
    name='NorsokJointCalculator',
)
