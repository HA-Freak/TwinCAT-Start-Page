@echo off
setlocal enabledelayedexpansion
pushd "%~dp0"

echo ============================================
echo  TwinCAT.XAE.StartPage - package build
echo ============================================
echo.
echo NOTE: bump the version FIRST (must be identical in all of):
echo   src\vsix\source.extension.vsixmanifest
echo   src\vsix\source.extension.cs
echo   TwinCAT.XAE.StartPage.Setup\Product.wxs
echo   TwinCAT.XAE.StartPage.Setup\TcPkg\*\*.nuspec
echo.

rem --- locate VS 2022 MSBuild (17.x - TcXaeShell64 is the VS 2022 shell,
rem     newer MSBuild deploys the extension to the wrong experimental hive)
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not exist "%VSWHERE%" (
    echo ERROR: vswhere.exe not found - is Visual Studio installed?
    exit /b 1
)

set "MSBUILD="
"%VSWHERE%" -products * -version "[17.0,18.0)" -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe > "%TEMP%\startpage-msbuild-path.txt"
set /p MSBUILD=<"%TEMP%\startpage-msbuild-path.txt"
del "%TEMP%\startpage-msbuild-path.txt" >nul 2>&1
if not defined MSBUILD (
    echo ERROR: Visual Studio 2022 MSBuild not found.
    exit /b 1
)
echo Using MSBuild: %MSBUILD%

rem resolve TcPkg via PATH first, then known install locations
set "TCPKG="
where tcpkg.exe > "%TEMP%\startpage-tcpkg-path.txt" 2>nul
set /p TCPKG=<"%TEMP%\startpage-tcpkg-path.txt"
del "%TEMP%\startpage-tcpkg-path.txt" >nul 2>&1
if not defined TCPKG set "TCPKG=%ProgramData%\Beckhoff\TcPkg\TcPkg.exe"
if not exist "%TCPKG%" set "TCPKG=%ProgramFiles%\Beckhoff\TcPkg\TcPkg.exe"
if not exist "%TCPKG%" (
    echo ERROR: TcPkg.exe not found - is the TwinCAT Package Manager installed?
    exit /b 1
)
echo Using TcPkg:   %TCPKG%

set "WIXTARGETS=%ProgramFiles(x86)%\MSBuild\Microsoft\WiX\v3.x\wix.targets"
if not exist "%WIXTARGETS%" (
    echo ERROR: WiX Toolset v3 MSBuild targets not found.
    exit /b 1
)

rem note: must NOT be named OUTDIR - MSBuild treats an OutDir environment
rem variable as a global property and would redirect the build outputs
set "PKGOUT=TwinCAT.XAE.StartPage.Setup\bin\Debug"

rem signing certificate in Cert:\CurrentUser\My (one-time setup: see
rem TwinCAT.XAE.StartPage.Setup\bin\Debug\signing.txt); override via SIGN_THUMBPRINT
if not defined SIGN_THUMBPRINT set "SIGN_THUMBPRINT=16FD1A0A1F625BFA1879CA7B9814D29AD88B5590"

set "PKGVERSION="
for /f "usebackq delims=" %%v in (`powershell -NoProfile -Command "([xml](Get-Content 'TwinCAT.XAE.StartPage.Setup\TcPkg\TwinCAT.XAE.StartPage\TwinCAT.XAE.StartPage.nuspec')).package.metadata.version"`) do set "PKGVERSION=%%v"
if not defined PKGVERSION (
    echo ERROR: could not read the package version from the nuspec.
    exit /b 1
)
echo Version:       %PKGVERSION%

echo.
echo --- [1/4] building extension (VSIX) ---
rem no PDB and neutral source paths, so the packages don't contain local paths
rem (user name); /t:Rebuild so no PDB from an earlier F5 build ends up in the VSIX
"%MSBUILD%" "src\vsix\TwinCAT.XAE.StartPage.csproj" /t:Rebuild /p:Configuration=Debug /p:DebugType=none /p:DebugSymbols=false /p:IncludeDebugSymbolsInVSIXContainer=false "/p:PathMap=%CD%=C:\src" /restore /v:minimal /nologo
if errorlevel 1 goto :fail

echo.
echo --- [2/4] building installer (MSI) ---
"%MSBUILD%" "TwinCAT.XAE.StartPage.Setup\TwinCAT.XAE.StartPage.Setup.wixproj" /p:Configuration=Debug /p:WixTargetsPath="%WIXTARGETS%" /v:minimal /nologo
if errorlevel 1 goto :fail

echo.
echo --- [3/4] packing TcPkg packages (nupkg) ---
for %%p in (
    TwinCAT.XAE.StartPage
    TwinCAT.XAE.StartPage.TcXaeShell64
    TwinCAT.XAE.StartPage.VS2022
    TwinCAT.XAE.StartPage.VS2026
) do (
    "%TCPKG%" pack "TwinCAT.XAE.StartPage.Setup\TcPkg\%%p\%%p.nuspec" -o "%PKGOUT%"
    if errorlevel 1 goto :fail
)

echo.
echo --- [4/4] signing TcPkg packages ---
for %%p in (
    TwinCAT.XAE.StartPage
    TwinCAT.XAE.StartPage.TcXaeShell64
    TwinCAT.XAE.StartPage.VS2022
    TwinCAT.XAE.StartPage.VS2026
) do (
    "%TCPKG%" sign "%PKGOUT%\%%p.%PKGVERSION%.nupkg" --timestamp-server None --thumbprint %SIGN_THUMBPRINT% --verify-after-signing
    if errorlevel 1 goto :fail
)

echo.
echo ============================================
echo  SUCCESS - packages in %PKGOUT%:
echo ============================================
dir /b "%PKGOUT%\*.nupkg"

popd
exit /b 0

:fail
echo.
echo BUILD FAILED
popd
exit /b 1
