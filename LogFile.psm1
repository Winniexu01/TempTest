Write-Host "Loading module LogFile"

enum LogType {
    Information
    Warning
    Error
}

$global:WarningCount = 0
$global:ErrorCount = 0

Function Write-LogMessage
{
    Param
    (
        [Parameter(Mandatory=$true)]
        [string] $Message,
        [Parameter(Mandatory=$false)]
        [LogType] $LogType = [LogType]::Information
    )

    switch ($LogType)
    {
        ([LogType]::Warning)     { Write-Warning "$Message"; $global:WarningCount++ }
        ([LogType]::Error)       { $Host.UI.WriteErrorLine("$Message"); $global:ErrorCount++ }
        ([LogType]::Information) { Write-Host "Information: $Message" }
    }
}

Function Set-TaskResult {
    if ($global:ErrorCount -gt 0) {
        Write-Host "##vso[task.complete result=Failed;]ERROR! The process ran into $global:ErrorCount error(s)."
    }
    elseif ($global:WarningCount -gt 0) {
        Write-Host "##vso[task.complete result=SucceededWithIssues;]WARNING! The process ran into $global:WarningCount warning(s)."
    }
    else {
        Write-Host "##vso[task.complete result=Succeeded;]Process completed successfully."
    }
}

Export-ModuleMember -Function Write-LogMessage, Set-TaskResult