$ErrorActionPreference = 'Stop'

# the extension identity from source.extension.vsixmanifest
$vsixId = '96b389fd-48b6-48d9-bbf2-ec6ebd026b14'

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) {
    Write-Warning 'vswhere.exe not found - skipping Visual Studio 2022 extension uninstall.'
    return
}

$vsixInstaller = & $vswhere -products * -version '[17.0,18.0)' -latest -find 'Common7\IDE\VSIXInstaller.exe'
if (-not $vsixInstaller) {
    Write-Warning 'Visual Studio 2022 not found - skipping extension uninstall.'
    return
}

# this package only manages the VS 2022 instances
[array]$instanceIds = & $vswhere -products * -version '[17.0,18.0)' -property instanceId
if ($instanceIds.Count -eq 0) {
    Write-Warning 'No Visual Studio 2022 instances found - skipping extension uninstall.'
    return
}

$idArg = $instanceIds -join ';'

$process = Start-Process -FilePath $vsixInstaller -ArgumentList "/quiet", "/instanceIds:$idArg", "/uninstall:$vsixId" -Wait -PassThru

# 0 = uninstalled, 1002 = extension is not installed, 2003 = not found
if ($process.ExitCode -notin 0, 1002, 2003) {
    throw "VSIXInstaller failed with exit code $($process.ExitCode)"
}
