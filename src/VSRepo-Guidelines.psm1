write-host "Loading module VSRepo-Guidelines"
$scriptDir = Split-Path $script:MyInvocation.MyCommand.Path
Import-Module -Name "$scriptDir\LogFile.psm1" -Force


Function SendReportEmail {
    param (
        [Parameter(Mandatory=$true)]
        [psobject] $email,
        [Parameter(Mandatory=$false)]
        [string] $method = '1'
    )

    # Force TLS1.2 when sending e-mail
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

    try {
        $dlls = @(
            "System.Diagnostics.DiagnosticSource.dll",
            "VSEng.Mailer.dll"
        )
        if ($method -eq '2') {
            Write-LogMessage -Message "Loading mailer assembly with method 2 (from script directory)"
            (Get-ChildItem -Path "$scriptDir\VSEng.Mailer" -Recurse -File -Include $dlls -ErrorAction SilentlyContinue | Where-Object { $_.FullName -like '*\net472\*' }).ForEach{
                Write-LogMessage -Message "Loading assembly: $($_.FullName)"
                [System.Reflection.Assembly]::LoadFrom($_.FullName)
            }
        }
        else {
            Write-LogMessage -Message "Loading mailer assembly with method 1 (from current directory)"
            (Get-ChildItem -Path $PWD.Path -Recurse -File -Include $dlls -ErrorAction SilentlyContinue).ForEach{
                Write-LogMessage -Message "Loading assembly: $($_.FullName)"
                [System.Reflection.Assembly]::LoadFrom($_.FullName)
            }
        }

        $mail_obj = [VSEng.Mailer.VSEngMailer]::CreateMailMessage($email.To, $email.Cc, "", $from, $email.Subject, $email.Body)
        [VSEng.Mailer.VSEngMailer]::SendMail($mail_obj)
    }
    catch {
        # we can't send an error email if email sending fails, so instead append to the log file.
        Write-LogMessage -Message "SendReportEmail: Exception type: $($_.Exception.GetType().FullName)" -LogType 'Error'
        Write-LogMessage -Message "Exception message: $($_.Exception.Message)" -LogType 'Error'
        Write-LogMessage -Message "Full exception: $($_.Exception.ToString())" -LogType 'Error'
    }
}

Function Test-Function() {
    Param (
        [Parameter(Mandatory=$false)]
        [string] $method = '2'
    )
    $body = "This is a test email to verify that the email sending function works correctly.<br><br>"
    $email = New-Object PSObject
    Add-Member -inputobject $email -membertype noteproperty -name To -value "v-wexu@microsoft.com"
    Add-Member -inputobject $email -membertype noteproperty -name Cc -value ""
    Add-Member -inputobject $email -membertype noteproperty -name Subject -Value "Test Email from PowerShell"
    Add-Member -inputobject $email -membertype noteproperty -name Body -Value $body
    SendReportEmail -email $email -method $method
}

Export-ModuleMember -Function Test-Function