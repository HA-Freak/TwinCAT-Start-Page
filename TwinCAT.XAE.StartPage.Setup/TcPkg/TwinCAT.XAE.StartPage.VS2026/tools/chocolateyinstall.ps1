$ErrorActionPreference = 'Stop'

$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$vsixPath = Join-Path $toolsDir 'TwinCAT.XAE.StartPage.vsix'

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) {
    throw 'vswhere.exe not found - is Visual Studio 2026 installed?'
}

$vsixInstaller = & $vswhere -products * -version '[18.0,19.0)' -latest -find 'Common7\IDE\VSIXInstaller.exe'
if (-not $vsixInstaller) {
    throw 'Visual Studio 2026 not found - cannot install the extension.'
}

# restrict to VS 2026 instances only - without /instanceIds, VSIXInstaller /quiet
# installs into every compatible instance
[array]$instanceIds = & $vswhere -products * -version '[18.0,19.0)' -property instanceId
if ($instanceIds.Count -eq 0) {
    throw 'No Visual Studio 2026 instances found.'
}

$idArg = $instanceIds -join ';'

Write-Host "Installing Start Page extension into Visual Studio 2026 instance(s): $idArg"

$process = Start-Process -FilePath $vsixInstaller -ArgumentList "/quiet", "/instanceIds:$idArg", "`"$vsixPath`"" -Wait -PassThru

# 0 = installed, 1001 = this version is already installed
if ($process.ExitCode -ne 0 -and $process.ExitCode -ne 1001) {
    throw "VSIXInstaller failed with exit code $($process.ExitCode)"
}
