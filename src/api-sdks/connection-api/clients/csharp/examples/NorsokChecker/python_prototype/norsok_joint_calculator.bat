@echo off
REM NORSOK Joint Calculator - standalone desktop app (pywebview, no HTTP port)
REM Prefer the "py" launcher: on Windows, plain python.exe/pythonw.exe on PATH
REM often resolve to the WindowsApps App Execution Alias stub (opens the Store
REM or silently no-ops) rather than a real interpreter. "py" is installed by
REM python.org's installer outside that alias and finds the real install via
REM the registry, so it works regardless of the user's PATH setup.
REM Ensures dependencies from requirements.txt are installed (no-op if already
REM satisfied) before launch, so a fresh machine doesn't fail with ModuleNotFoundError.
REM Runs headless (no console window). A startup crash (traceback) is captured
REM to error.log next to this file so failures are not lost silently.
cd /d "%~dp0"
where py >nul 2>nul
if %errorlevel%==0 (
	set PY=py
	set PYW=pyw
) else (
	set PY=python
	set PYW=pythonw
)
%PY% -m pip install -r requirements.txt --quiet --disable-pip-version-check
if %errorlevel% neq 0 (
	echo Failed to install dependencies. Is Python installed and on PATH? > error.log
	pause
	exit /b 1
)
start "" %PYW% app.py 2>error.log
