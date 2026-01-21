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