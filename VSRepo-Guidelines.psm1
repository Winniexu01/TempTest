write-host "Loading module VSRepo-Guidelines"
$scriptDir = Split-Path $script:MyInvocation.MyCommand.Path
Import-Module -Name "$scriptDir\LogFile.psm1" -Force

Function Test-LogFileModule {
    Param(
        [Parameter(Mandatory=$false)]
        [string]$LogType = 'Information'
    )
    Write-Host "Testing LogFile module..."
    Write-LogMessage -Message "This is a $LogType message." -LogType $LogType
    Set-TaskResult
}

Export-ModuleMember -Function Test-LogFileModule