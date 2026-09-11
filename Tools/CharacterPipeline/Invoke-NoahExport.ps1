param(
    [int]$TimeoutSeconds = 120
)

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$Blender = 'C:\Program Files\Blender Foundation\Blender 5.0\blender.exe'
$Source = Join-Path $RepoRoot 'Assets\_InsideTheWalls\Character Models\character-inmate-01-T_Rigged_4652083419\character-inmate-01-T_Rigged_4652083419\character_publish.blend'
$OutputDirectory = Join-Path $RepoRoot 'Assets\_InsideTheWalls\Art\Characters\Principals\NoahMercer\Source\Interchange'
$Output = Join-Path $OutputDirectory 'NoahMercer_SourceRig.fbx'
$Report = Join-Path $OutputDirectory 'NoahMercer_SourceRig.audit.json'
$Script = Join-Path $PSScriptRoot 'noah_export.py'

if ($TimeoutSeconds -lt 30 -or $TimeoutSeconds -gt 300) {
    throw 'TimeoutSeconds must be between 30 and 300.'
}

foreach ($RequiredPath in @($Blender, $Source, $Script)) {
    if (-not (Test-Path -LiteralPath $RequiredPath -PathType Leaf)) {
        throw "Required file not found: $RequiredPath"
    }
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$Stage = Join-Path $OutputDirectory ('.staging-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $Stage | Out-Null
$StageOutput = Join-Path $Stage 'NoahMercer_SourceRig.fbx'
$StageReport = Join-Path $Stage 'NoahMercer_SourceRig.audit.json'
$BeforeHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $Source).Hash
$Info = [System.Diagnostics.ProcessStartInfo]::new()
$Info.FileName = $Blender
$Info.UseShellExecute = $false
$Info.CreateNoWindow = $true
$Info.RedirectStandardOutput = $true
$Info.RedirectStandardError = $true
foreach ($Argument in @('--factory-startup', '--background', '--python', $Script, '--', $Source, $StageOutput, $StageReport)) {
    [void]$Info.ArgumentList.Add($Argument)
}
$Process = [System.Diagnostics.Process]::new()
$Process.StartInfo = $Info
[void]$Process.Start()
$StdOut = $Process.StandardOutput.ReadToEndAsync()
$StdErr = $Process.StandardError.ReadToEndAsync()

if (-not $Process.WaitForExit($TimeoutSeconds * 1000)) {
    Stop-Process -Id $Process.Id -Force
    Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
    throw "Blender export exceeded $TimeoutSeconds seconds; process $($Process.Id) was terminated and partial outputs removed."
}

if ($Process.ExitCode -ne 0) {
    $ErrorText = $StdErr.Result
    Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
    throw "Blender export failed with exit code $($Process.ExitCode): $ErrorText"
}

$AfterHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $Source).Hash
if ($BeforeHash -ne $AfterHash) {
    Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
    throw 'Source hash changed during export; outputs removed.'
}

if (-not (Test-Path -LiteralPath $StageOutput -PathType Leaf) -or -not (Test-Path -LiteralPath $StageReport -PathType Leaf)) {
    Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
    throw 'Blender exited successfully without both expected outputs.'
}

$OutputRoot = [IO.Path]::GetFullPath($OutputDirectory) + [IO.Path]::DirectorySeparatorChar
foreach ($Candidate in @($StageOutput, $StageReport)) {
    if (-not [IO.Path]::GetFullPath($Candidate).StartsWith($OutputRoot, [StringComparison]::OrdinalIgnoreCase)) {
        Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
        throw "Staged output escaped destination: $Candidate"
    }
}

$Audit = Get-Content -Raw -LiteralPath $StageReport | ConvertFrom-Json
if ($Audit.source_sha256 -ne $BeforeHash -or $Audit.export.settings_version -ne '1.1' -or $Audit.export.bytes -le 0) {
    Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
    throw 'Staged export validation failed; prior canonical outputs remain untouched.'
}
$TextureEntries = @($Audit.texture_manifest)
$TexturePaths = @($TextureEntries | ForEach-Object { $_.path } | Select-Object -Unique)
$TextureHashes = @($TextureEntries | ForEach-Object { $_.sha256 } | Select-Object -Unique)
$TextureSemantics = @($TextureEntries | ForEach-Object { $_.semantic } | Select-Object -Unique)
if ($TextureEntries.Count -ne 3 -or $TexturePaths.Count -ne 3 -or $TextureHashes.Count -ne 3 -or @($TextureSemantics | Where-Object { $_ -in @('BaseColor', 'Roughness', 'Normal') }).Count -ne 3) {
    Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
    throw 'Texture handoff validation failed: expected unique BaseColor, Roughness, and Normal files.'
}
foreach ($Texture in $TextureEntries) {
    $StagedTexture = Join-Path $Stage $Texture.path
    if (-not (Test-Path -LiteralPath $StagedTexture -PathType Leaf) -or (Get-FileHash -Algorithm SHA256 -LiteralPath $StagedTexture).Hash -ne $Texture.sha256) {
        Remove-Item -LiteralPath $Stage -Recurse -Force -ErrorAction SilentlyContinue
        throw "Texture manifest validation failed: $($Texture.path)"
    }
}

Move-Item -LiteralPath $StageOutput -Destination $Output -Force
Move-Item -LiteralPath $StageReport -Destination $Report -Force
$TextureDestination = Join-Path $OutputDirectory 'Textures'
if (Test-Path -LiteralPath (Join-Path $Stage 'Textures')) {
    New-Item -ItemType Directory -Force -Path $TextureDestination | Out-Null
    Get-ChildItem -LiteralPath (Join-Path $Stage 'Textures') -File | ForEach-Object { Move-Item -LiteralPath $_.FullName -Destination (Join-Path $TextureDestination $_.Name) -Force }
}
Remove-Item -LiteralPath $Stage -Recurse -Force

$LogText = (($StdOut.Result + "`n" + $StdErr.Result) -replace "`0", '')
if ($LogText.Length -gt 20000) { $LogText = $LogText.Substring($LogText.Length - 20000) }
Set-Content -LiteralPath (Join-Path $OutputDirectory 'NoahMercer_SourceRig.blender.log') -Value $LogText -Encoding utf8
Get-Content -Raw -LiteralPath $Report
