function Test-Function() {
    $piPackageRoot = Join-Path $env:AssemblyPath 'VSEng.PI'

    if (Test-Path $piPackageRoot -PathType Container) {
        $piPackage = Get-ChildItem -Path $piPackageRoot -Directory -ErrorAction Stop |
            Where-Object { $_.Name -as [version] } |
            Sort-Object { [version]$_.Name } -Descending |
            Select-Object -First 1
    }

    $binDir = Join-Path $piPackage 'bin'

    if (-not (Test-Path $binDir -PathType Container)) {
        Write-LogMessage -Message "Bin directory not found at expected location: $binDir" -LogType 'Error'
        throw "Bin directory not found at expected location: $binDir"
    }
}