write-host "Loading module VSRepo-Guidelines"
$scriptDir = Split-Path $script:MyInvocation.MyCommand.Path
Import-Module -Name "$scriptDir\LogFile.psm1" -Force

Function Test-LogFileModule {
    Param(
        [Parameter(Mandatory=$false)]
        [string]$LogType = 'Information'
    )
                    Write-LogMessage -Message "1Policy Url: $($policy.PolicyUrl)
             Policy ID: $($policy.PolicyId)" -LogType $LogType
    Write-LogMessage -Message "2Policy Url: `r`nPolicy ID: " -LogType $LogType
    Write-LogMessage -Message "Error" -LogType 'Error'
    Set-TaskResult
}

Export-ModuleMember -Function Test-LogFileModule