@echo off

set "scriptPath=%~dp0"
set "folderPath=%scriptPath%Stored Procedures"
set "outputFile=%scriptPath%stored_procedures.sql"

setlocal enabledelayedexpansion

for %%F in ("%folderPath%\*.sql") do (
    set "spName=%%~nF"
    echo. >> "%outputFile%"
    echo -- Stored Procedure: !spName! >> "%outputFile%"
    type "%%F" >> "%outputFile%"
    echo. >> "%outputFile%"
)

endlocal

echo.
echo Script generation completed! The script file is located at:
echo %outputFile%
pause