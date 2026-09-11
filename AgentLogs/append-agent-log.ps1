param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9]{4}-[0-9]{2}-[0-9]{2}--[a-z0-9-]+--[a-z0-9-]+\.md$')]
    [string]$LogName,

    [Parameter(Mandatory = $true)]
    [ValidateSet('Assignment', 'Progress', 'Decision', 'Feedback', 'Verification', 'Handoff')]
    [string]$Section,

    [Parameter(Mandatory = $true)]
    [string]$Message
)

$logRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$logPath = Join-Path $logRoot $LogName
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss K'
$entry = "`r`n## $Section - $timestamp`r`n`r`n$Message`r`n"
Add-Content -LiteralPath $logPath -Value $entry -Encoding UTF8

Write-Output $logPath
