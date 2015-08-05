REM Creates a Release
REM %1 - Directory to output release to - defaults to Labs\Release
@Echo On
REM Remove any previous releases
if not exist CreateRelease.cmd goto :WrongDir
if exist Release rd Release /s /q
md Release

pushd .
cd Microsoft.Activities
call CreateRelease.cmd
popd 

pushd .
cd Microsoft.Activities.UnitTesting
call CreateRelease.cmd
popd
GOTO :EOF
:WrongDir
@Echo Invoke CreateRelease from the Labs folder
