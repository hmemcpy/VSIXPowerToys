@echo off
pushd %~dp0
call .\Tools\elevate_me.bat %0
srm.exe uninstall VSIXPowerToys.Shell.dll
pause