@echo off
set dotNetBasePath=%windir%\Microsoft.NET\Framework
if exist %dotNetBasePath%64 set dotNetBasePath=%dotNetBasePath%64
for /R %dotNetBasePath% %%i in (*msbuild.exe) do set msbuild=%%i

set target=Dixie.sln
%msbuild% /t:Rebuild /maxcpucount /v:m /p:WarningLevel=0 /p:Configuration=Release %target%

