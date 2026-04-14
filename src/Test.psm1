function Test-Function() {
    $piPackageRoot = Join-Path $env:AssemblyPath 'VSEng.PI'
    $piPackage = $piPackageRoot

    if (Test-Path $piPackageRoot -PathType Container) {
        $versionDirectories = Get-ChildItem -Path $piPackageRoot -Directory -ErrorAction Stop |
            Where-Object { $_.Name -as [version] } |
            Sort-Object { [version]$_.Name } -Descending

        if ($versionDirectories.Count -gt 0) {
            $piPackage = $versionDirectories[0].FullName
        }
    }

    $binDir = Join-Path $piPackage 'bin'

    Write-Host "Looking for VSEng.Mailer.dll in: $binDir"
}