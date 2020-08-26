@ECHO OFF
SETLOCAL EnableDelayedExpansion

:: msbuild path
SET MSBuild="%programfiles(x86)%\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"

:: msbuild params
SET MSBuildParam=
SET MSBuildParam=%MSBuildParam% /maxcpucount
SET MSBuildParam=%MSBuildParam% /nologo
SET MSBuildParam=%MSBuildParam% /nodeReuse:false
SET MSBuildParam=%MSBuildParam% /property:AllowedReferenceRelatedFileExtensions=none
SET MSBuildParam=%MSBuildParam% /property:Configuration=Release
SET MSBuildParam=%MSBuildParam% /target:clean,restore,build
SET MSBuildParam=%MSBuildParam% /verbosity:minimal

:: execute msbuild
FOR %%a IN ("%~dp0\*.sln") DO (
    %MSBuild% %MSBuildParam% %%~fa
    IF NOT %errorlevel%==0 PAUSE
)

ENDLOCAL
EXIT /b