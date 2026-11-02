$ErrorActionPreference = 'Stop'

$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

$packageArgs = @{
    packageName    = $env:ChocolateyPackageName
    fileType       = 'msi'
    file           = Join-Path $toolsDir 'TwinCAT.XAE.StartPage.msi'
    silentArgs     = "/qn /norestart RUNFROMTCPKG=1 /l*v `"$($env:TEMP)\TwinCAT.XAE.StartPage.install.log`""
    validExitCodes = @(0, 3010, 1641)
}

Install-ChocolateyInstallPackage @packageArgs
