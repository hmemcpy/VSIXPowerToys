@echo off
cd %~dp0
call .\Tools\elevate_me.bat %0
.\Tools\srm.exe install VSIXPowerToys.Shell.dll -codebase
pause