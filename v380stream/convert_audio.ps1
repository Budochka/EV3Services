# Convert audio.raw to audio.wav
# Usage: .\convert_audio.ps1 -InputFile "path\to\audio.raw"
#   or:  .\convert_audio.ps1 "path\to\audio.raw"
#   or:  .\convert_audio.ps1  (will look for audio.raw in current/script dir)

param(
    [Parameter(Position=0, ValueFromPipeline=$false)]
    [string]$InputFile = "",
    [string]$OutputFile = "",
    [int]$SkipBytes = 20  # Default: 20 bytes (Node.js approach, works perfectly). Use -SkipBytes 18 for C++ FLV approach
)

$frameSize = 272

# If InputFile not provided, try to find audio.raw
if ([string]::IsNullOrEmpty($InputFile)) {
    $InputFile = "audio.raw"
}

# Resolve input file path - try absolute path, then current directory, then script directory
if (-not [IO.Path]::IsPathRooted($InputFile)) {
    # Relative path - check current directory first
    if (Test-Path $InputFile) {
        $InputFile = (Resolve-Path $InputFile).Path
    } else {
        # Check script directory
        $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
        $scriptPath = Join-Path $scriptDir $InputFile
        if (Test-Path $scriptPath) {
            $InputFile = (Resolve-Path $scriptPath).Path
        } else {
            Write-Host "Error: Cannot find '$InputFile' in current directory or script directory"
            Write-Host "Current directory: $(Get-Location)"
            Write-Host "Script directory: $scriptDir"
            Write-Host ""
            Write-Host "Usage: .\convert_audio.ps1 -InputFile `"path\to\audio.raw`""
            Write-Host "   or: .\convert_audio.ps1 `"path\to\audio.raw`""
            exit 1
        }
    }
} else {
    # Absolute path provided - just verify it exists
    if (-not (Test-Path $InputFile)) {
        Write-Host "Error: Input file not found: $InputFile"
        exit 1
    }
    $InputFile = (Resolve-Path $InputFile).Path
}

# Resolve output file path - if not specified, use same directory and name as input but .wav
if ([string]::IsNullOrEmpty($OutputFile)) {
    $OutputFile = [IO.Path]::ChangeExtension($InputFile, ".wav")
} elseif (-not [IO.Path]::IsPathRooted($OutputFile)) {
    # Relative output path - use same directory as input
    $OutputFile = Join-Path (Split-Path $InputFile) $OutputFile
} else {
    # Absolute path provided - use as-is (output file doesn't need to exist yet)
    # Just normalize it
    $OutputFile = [IO.Path]::GetFullPath($OutputFile)
}

Write-Host "Processing $InputFile..."
Write-Host "Output will be: $OutputFile"
Write-Host ""

if (-not (Test-Path $InputFile)) {
    Write-Host "Error: Input file not found: $InputFile"
    exit 1
}

$bytes = [IO.File]::ReadAllBytes($InputFile)
$frames = [Math]::Floor($bytes.Length / $frameSize)
Write-Host "Found $frames frames of $frameSize bytes each (total: $($bytes.Length) bytes)"
Write-Host ""

# Analyze first frame
Write-Host "First frame analysis (bytes 0-271):"
$first20 = $bytes[0..19] -join ' ' -replace '(\d+)', { '{0:X2}' -f [int]$args[0] }
$first20dec = ($bytes[0..19] | ForEach-Object { "$_" }) -join ' '
Write-Host "  Bytes 0-19 (hex): $first20"
Write-Host "  Bytes 0-19 (dec): $first20dec"

# Check if first bytes are zeros
$allZeros = ($bytes[0..19] | Where-Object { $_ -ne 0 }).Count -eq 0
if ($allZeros) {
    Write-Host "  ⚠️  First 20 bytes are ALL ZEROS - might be header padding"
    Write-Host "  Checking bytes 20-39..."
    $next20 = $bytes[20..39] -join ' ' -replace '(\d+)', { '{0:X2}' -f [int]$args[0] }
    Write-Host "  Bytes 20-39 (hex): $next20"
    $next20zeros = ($bytes[20..39] | Where-Object { $_ -ne 0 }).Count -eq 0
    if ($next20zeros) {
        Write-Host "  ⚠️  Bytes 20-39 also zeros - checking where data starts..."
        # Find first non-zero byte
        $firstNonZero = -1
        for ($i = 0; $i -lt [Math]::Min(100, $bytes.Length); $i++) {
            if ($bytes[$i] -ne 0) {
                $firstNonZero = $i
                break
            }
        }
        if ($firstNonZero -ge 0) {
            Write-Host "  First non-zero byte at offset: $firstNonZero"
        } else {
            Write-Host "  ⚠️  No non-zero bytes found in first 100 bytes!"
        }
    }
}

Write-Host ""
Write-Host "Using skip size: $SkipBytes bytes (default: 20 for optimal quality, 18 also works well)"
Write-Host ""

# Process with the specified skip size
$output = New-Object System.Collections.Generic.List[byte]

for ($i = 0; $i -lt $frames; $i++) {
    $offset = $i * $frameSize + $SkipBytes
    $remaining = $frameSize - $SkipBytes
    if ($offset + $remaining -le $bytes.Length) {
        for ($j = 0; $j -lt $remaining; $j++) {
            $output.Add($bytes[$offset + $j])
        }
    }
}

$strippedFile = Join-Path (Split-Path $InputFile) "audio_stripped_temp.raw"
[IO.File]::WriteAllBytes($strippedFile, $output.ToArray())
Write-Host "Created $strippedFile with $($output.Count) bytes (skipped $SkipBytes bytes from each frame)"

$nonZeroCount = ($output | Where-Object { $_ -ne 0 }).Count
Write-Host "Non-zero bytes: $nonZeroCount / $($output.Count) ($([Math]::Round($nonZeroCount * 100.0 / $output.Count, 2))%)"

Write-Host ""
Write-Host "Converting to WAV with volume normalization..."

# Convert with SoX and normalize volume to avoid low volume issue
$soxCmd = "sox -t ima -r 8000 -e ms-adpcm `"$strippedFile`" -e signed-integer -b 16 -V1 `"$OutputFile`" gain -n -3 2>&1"
Write-Host "Command: sox -t ima -r 8000 -e ms-adpcm [stripped] -e signed-integer -b 16 -V1 [output] gain -n -3"
$soxResult = & cmd /c $soxCmd

if ($LASTEXITCODE -eq 0 -and (Test-Path $OutputFile)) {
    $wavSize = (Get-Item $OutputFile).Length
    Write-Host "✓ Success! Created $OutputFile ($wavSize bytes)"
    Write-Host "  Volume normalized with gain -n -3 (prevents clipping while increasing volume)"
    Remove-Item $strippedFile -ErrorAction SilentlyContinue
} else {
    Write-Host "✗ SoX conversion failed: $($soxResult -join ' ')"
    Write-Host "`nTrying without normalization..."
    $soxCmd2 = "sox -t ima -r 8000 -e ms-adpcm `"$strippedFile`" -e signed-integer -b 16 `"$OutputFile`" 2>&1"
    $soxResult2 = & cmd /c $soxCmd2
    if ($LASTEXITCODE -eq 0 -and (Test-Path $OutputFile)) {
        Write-Host "✓ Success (without normalization): $OutputFile"
        Remove-Item $strippedFile -ErrorAction SilentlyContinue
    } else {
        Write-Host "✗ Both attempts failed. Stripped file saved as: $strippedFile"
    }
}

# OLD TESTING CODE - commented out, but can be re-enabled if needed
<#
Write-Host "Testing different skip sizes..."

# Test skip sizes: 16, 18, 20
$skipSizes = @(16, 18, 20)
$testFiles = @{}

foreach ($skipBytes in $skipSizes) {
    Write-Host "`nProcessing with skip=$skipBytes bytes..."
    $output = New-Object System.Collections.Generic.List[byte]
    
    for ($i = 0; $i -lt $frames; $i++) {
        $offset = $i * $frameSize + $skipBytes
        $remaining = $frameSize - $skipBytes
        if ($offset + $remaining -le $bytes.Length) {
            for ($j = 0; $j -lt $remaining; $j++) {
                $output.Add($bytes[$offset + $j])
            }
        }
    }
    
    $strippedFile = "audio_skip$skipBytes.raw"
    [IO.File]::WriteAllBytes($strippedFile, $output.ToArray())
    $testFiles[$skipBytes] = $strippedFile
    Write-Host "  Created $strippedFile with $($output.Count) bytes"
    
    # Check if stripped file has non-zero data
    $nonZeroCount = ($output | Where-Object { $_ -ne 0 }).Count
    Write-Host "  Non-zero bytes: $nonZeroCount / $($output.Count) ($([Math]::Round($nonZeroCount * 100.0 / $output.Count, 2))%)"
}

# OLD TESTING CODE - END
#>
