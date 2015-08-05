REM Builds All Labs binaries, docs and nuget packages
@Echo Off
msbuild labs.proj /p:Configuration=Release /fl
@Echo Run TestLabs to unit test
