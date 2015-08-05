if "%1" == "" GOTO ALL
gacutil -u  "ActivityLibrary1,Version=%1.0.0.0"
GOTO :SHOW
:ALL
gacutil -u ActivityLibrary1
:SHOW
gacutil -l ActivityLibrary1