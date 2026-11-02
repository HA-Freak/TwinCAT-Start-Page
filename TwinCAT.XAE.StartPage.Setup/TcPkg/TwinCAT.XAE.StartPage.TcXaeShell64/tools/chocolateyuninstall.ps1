$ErrorActionPreference = 'Stop'

# the MSI uses an auto-generated ProductCode per build, so locate the install
# via its DisplayName (Product/@Name in Product.wxs); the old pre-rename
# DisplayName is included so installs of older versions can be removed too
[array]$keys = Get-UninstallRegistryKey -SoftwareName 'TwinCAT.XAE.StartPage'
if ($keys.Count -eq 0) {
    [array]$keys = Get-UninstallRegistryKey -SoftwareName 'TwinCATStartPagex64'
}

if ($keys.Count -eq 0) {
    Write-Warning "$($env:ChocolateyPackageName) is not installed."
    return
}

foreach ($key in $keys) {
    $packageArgs = @{
        packageName    = $env:ChocolateyPackageName
        fileType       = 'msi'
        file           = ''
        silentArgs     = "$($key.PSChildName) /qn /norestart"
        validExitCodes = @(0, 3010, 1605, 1614, 1641)
    }

    Uninstall-ChocolateyPackage @packageArgs
}
