sc create RTMPSERVER binpath=%~dp0RTMPServer.exe
Net Start RTMPSERVER 
sc config RTMPSERVER start=auto
pause

