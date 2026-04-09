write-host "Loading module VSRepo-Guidelines"
$scriptDir = Split-Path $script:MyInvocation.MyCommand.Path
Import-Module -Name "$scriptDir\LogFile.psm1" -Force

Function Test-Module {
    Write-Host "Script directory: $scriptDir"
    Write-Host "Working directory: $(Get-Location)"
    Write-Host "System default working directory: $env:SYSTEM_DEFAULTWORKINGDIRECTORY"
}

Export-ModuleMember -Function Test-Module