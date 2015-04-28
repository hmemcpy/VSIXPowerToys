@echo off
cd %~dp0
call .\Tools\elevate_me.bat %0
.\Tools\srm.exe uninstall VSIXPowerToys.Shell.dll
pause