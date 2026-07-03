@echo off
REM NORSOK Joint Calculator - standalone desktop app (pywebview, no HTTP port)
REM Runs via pythonw.exe = no console window. A startup crash (traceback) is
REM captured to error.log next to this file so failures are not lost silently.
cd /d "%~dp0"
start "" pythonw.exe app.py 2>error.log
