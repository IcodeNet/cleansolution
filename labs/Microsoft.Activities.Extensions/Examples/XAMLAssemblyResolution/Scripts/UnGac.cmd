@Echo Off
Echo UnGac removes ActivityLibrary1 from GAC with optional version
if "%1"=="" GOTO ALL
gacutil -u ActivityLibrary1,Version=%1.0.0.0,Culture=neutral,PublicKeyToken=c18b97d2d48a43ab
GOTO :END
:ALL
gacutil -u ActivityLibrary1
:END
@Echo ---- ActivityLibrary1 in GAC ----
gacutil -l ActivityLibrary1