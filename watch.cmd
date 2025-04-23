@echo off
:loop
dotnet build
timeout /t 1 >nul
goto loop
