@echo off
pushd %~dp0
call .\Tools\elevate_me.bat %0
srm.exe install VSIXPowerToys.Shell.dll -codebase
pause