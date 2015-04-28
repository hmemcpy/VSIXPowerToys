call :is_elevated is_elevated
if {%is_elevated%} EQU {0} .\Tools\elevate.cmd %~nx1 & exit /b 0

:is_elevated    -- Returns whether this batch is elevated
::                 -- %~1: Set to 1 if elevated, 0 otherwise
SETLOCAL

:: method (applicable to Windows 7, and maybe Vista)
::  try to write a zero-byte file to a system directory
::    if successful, we are in Elevated mode and delete the file
::    if unsuccessful, avoid the "Access is denied" message

:: arbitrary choice of system directory and filename
set tst="%windir%\$del_me$"

:: the first brackets are required to avoid getting the message,
::   even though 2 is redirected to nul.  no, I don't know why.
(type nul>%tst%) 2>nul && (del %tst% & set elev=t) || (set elev=)

if defined elev (set is_elevated=1) else (set is_elevated=0)

(ENDLOCAL & REM -- RETURN VALUES
    IF "%~1" NEQ "" SET %~1=%is_elevated%
)
GOTO:EOF