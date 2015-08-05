@Echo Off
@Echo DO NOT PUBLISH PACKAGES UNITL TESTED
REM @Echo NuGet Packages To Publish
REM dir /b Nuget.TestSource\*.nupkg
@Echo Press Ctrl+C to stop publishing, otherwise press any key to publish
pause
for /f %%a IN ('dir /b Release\Microsoft.Activities\*.nupkg') do nuget push -source http://packages.nuget.org/v1/ Release\Microsoft.Activities\%%a 5c6bacea-7a51-4a7a-b90c-948eafd620b4
for /f %%a IN ('dir /b Release\Microsoft.Activities.UnitTesting\*.nupkg') do nuget push -source http://packages.nuget.org/v1/ Release\Microsoft.Activities.UnitTesting\%%a 5c6bacea-7a51-4a7a-b90c-948eafd620b4
