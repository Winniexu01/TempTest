Write-Host "Loading module LogFile"

enum LogType {
    Information
    Warning
    Error
}

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
        'Warning'     { Write-Warning "$Message" }
        'Error'       { $Host.UI.WriteErrorLine("$Message") }
        default       { Write-Host "Information: $Message" }
    }

    switch ($LogType)
    {
        ([LogType]::Warning)     { Write-Warning "$Message" }
        ([LogType]::Error)       { $Host.UI.WriteErrorLine("$Message") }
        ([LogType]::Information) { Write-Host "Information: $Message" }
    }

    switch ($LogType)
    {
        ([LogType]::Warning)     { Write-Warning "$Message" }
        ([LogType]::Error)       { Write-Error "$Message" }
        ([LogType]::Information) { Write-Host "Information: $Message" }
    }
    Write-Host "Next step."
}

Export-ModuleMember -Function Write-LogMessage