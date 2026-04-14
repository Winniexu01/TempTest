function Test-Function() {
    Write-Host "Looking for VSEng.PI packages in: $env:VSEngPIPath"
    if (Test-Path $env:VSEngPIPath -PathType Container) {
        $piPackage = Get-ChildItem -Path $env:VSEngPIPath -Directory -ErrorAction Stop |
            Where-Object { $_.Name -as [version] } |
            Sort-Object { [version]$_.Name } -Descending |
            Select-Object -First 1
    }
    Write-Host "Found VSEng.PI package: $($piPackage.FullName)"
    $binDir = Join-Path $piPackage.FullName 'bin'
    Write-Host "Looking for bin directory at: $binDir"
    if (-not (Test-Path $binDir -PathType Container)) {
        Write-LogMessage -Message "Bin directory not found at expected location: $binDir" -LogType 'Error'
        throw "Bin directory not found at expected location: $binDir"
    }
}