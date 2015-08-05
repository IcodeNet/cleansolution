@Echo Off
REM Build Release Package
set ASSEMBLY=Microsoft.Activities.UnitTesting
set SRC=..\%ASSEMBLY%
set NUGET=..\NuGet.TestSource
set REL=..\Release\%ASSEMBLY%
set REL1=..\Release\%ASSEMBLY%.PU1
set TEMP=%REL%\Temp
set TEMP1=%REL1%\Temp
if exist %REL% rd %REL% /s /q
if exist %REL1% rd %REL1% /s /q
md %REL%
md %REL1%

REM xcopy %SRC%\%ASSEMBLY%\bin\Release %TEMP% /i
xcopy %SRC%\%ASSEMBLY%.Design\bin\Release %TEMP% /i
xcopy %SRC%\%ASSEMBLY%.NuGet\Package\Help\*.chm %TEMP% /i

xcopy %SRC%\%ASSEMBLY%.Design\bin\Release.PU1 %TEMP1% /i

del %TEMP%\*.xml
del %TEMP1%\*.xml

..\..\..\..\tools\CreatePackageCmd %REL% %ASSEMBLY%.dll
..\..\..\..\tools\CreatePackageCmd %REL1% %ASSEMBLY%.dll

call %REL%\buildpackage.cmd
call %REL1%\buildpackage.cmd

pushd .
cd %REL1%
REM ren %REL1%\%ASSEMBLY%.* %REL1%\%ASSEMBLY%.PU1.* 
ren %ASSEMBLY%.* %ASSEMBLY%.PU1.* 
popd

xcopy %NUGET%\%ASSEMBLY%.* %REL%
del %REL%\%ASSEMBLY%.UnitTesting.*

REM call %SRC%\DeployToSamples.cmd