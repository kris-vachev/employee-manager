@echo off

cd /d "%~dp0Scripts\Run Scripts"
START "" "RUN_API.bat"
CALL "RUN_UI.bat"