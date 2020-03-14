@echo off

set DIR_NAME=app-%~1

if exist "%DIR_NAME%" (
	echo This version already exists
	exit /B -1
)

MD "%DIR_NAME%"

start dotnet publish -o "%DIR_NAME%/bin/api" -c Release BackendApi/BackendApi.csproj
start dotnet publish -o "%DIR_NAME%/bin/client" -c Release BackendClient/BackendClient.csproj
start dotnet publish -o "%DIR_NAME%/bin/logger" -c Release Logger/Logger.csproj

MD "%DIR_NAME%/config"
xcopy "BackendClient/config" "%DIR_NAME%/config"

@echo start bin/api/BackendApi.exe > "%DIR_NAME%/start.bat"
@echo start bin/client/BackendClient.exe >> "%DIR_NAME%/start.bat"
@echo start bin/logger/Logger.exe >> "%DIR_NAME%/start.bat"

@echo taskkill /IM BackendApi.exe > "%DIR_NAME%/stop.bat"
@echo taskkill /IM BackendClient.exe >> "%DIR_NAME%/stop.bat"
@echo taskkill /IM Logger.exe >> "%DIR_NAME%/stop.bat"
 