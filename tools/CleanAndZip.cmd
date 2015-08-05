REM Clean and Zip
REM Cleans a project and creates a zip file for distribution
REM Requires  7z.exe (7Zip) in the path C:\Program Files\7-Zip (http://www.7-zip.org/)
REM Requires CleanProject.exe in the path
REM %1 - Solution Name
REM %2 - Solution Folder (optional if different)

@Echo Off
@Echo CleanAndZip

if "%1"=="" GOTO :BadArgs

SET SOLUTION=%1

if "%2"=="" (SET SOLUTIONFOLDER=%SOLUTION%) ELSE (SET SOLUTIONFOLDER=%2%)

@Echo Cleaning and Zipping Solution %SOLUTION%

REM Remove the previous temp if exists
if exist "%TEMP%\%SOLUTION%" rd "%TEMP%\%SOLUTION%" /s /q
REM Copy to Temp folder

xcopy "%SOLUTIONFOLDER%" "%TEMP%\%SOLUTION%" /s /i

cleanproject /Q /R /D:"%TEMP%\%SOLUTION%"

7z a %SOLUTION%.zip "%TEMP%\%SOLUTION%" -r

GOTO :EOF
:BadArgs
@Echo CleanAndZip.cmd (SolutionName) (Solution Folder)
pause
