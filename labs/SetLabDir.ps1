$myDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Write-Host Setting LABDIR to $myDir
[Environment]::SetEnvironmentVariable("LABDIR", $myDir, "Machine")
