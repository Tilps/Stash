<#
.SYNOPSIS
    Re-evaluates gathered Sudoku puzzles (in the 1.x.y and 2 buckets) using the updated solver.

.DESCRIPTION
    Runs the multi-threaded Sudoku.Evaluator to solve gathered puzzles and report their new ratings,
    detect rating shifts (easier / harder / unchanged), and optionally export full results to CSV.

.PARAMETER File
    Specific file name or pattern to evaluate (e.g., '2.txt', '1.1.3.txt', '1.1.*').
    Defaults to all 1.x.y and 2 bucket files in GatheredSudokus.

.PARAMETER Limit
    Maximum number of puzzles to evaluate per file (useful for fast sampling).

.PARAMETER Lookahead
    Maximum lookahead depth allowed (default: 2).

.PARAMETER Threads
    Degree of parallelism (defaults to system CPU logical core count).

.PARAMETER Csv
    Path to save complete evaluation results as CSV.

.PARAMETER SummaryOnly
    Display only summary statistics and breakdown tables, suppressing the detailed puzzle list.

.PARAMETER Verbose
    Print individual puzzle evaluation progress in real time.

.PARAMETER Timeout
    Timeout per puzzle in milliseconds (default: 30000).

.EXAMPLE
    .\evaluate-gathered.ps1
    Evaluates all 1.x.y and 2 gathered puzzle buckets across all available CPU threads.

.EXAMPLE
    .\evaluate-gathered.ps1 -File "2.txt"
    Evaluates only the puzzles in 2.txt.

.EXAMPLE
    .\evaluate-gathered.ps1 -File "1.1.*" -Csv "rating_shifts.csv"
    Evaluates all 1.1.* files and exports results to rating_shifts.csv.
#>

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$File,

    [int]$Limit = 0,

    [int]$Lookahead = 2,

    [int]$Threads = [Environment]::ProcessorCount,

    [string]$Csv,

    [switch]$SummaryOnly,

    [switch]$VerboseOutput,

    [int]$Timeout = 30000
)

$ErrorActionPreference = "Stop"

$projectPath = Join-Path $PSScriptRoot "Sudoku.Evaluator\Sudoku.Evaluator.csproj"
if (-not (Test-Path $projectPath)) {
    Write-Error "Could not find Sudoku.Evaluator project at $projectPath"
    exit 1
}

$cliArgs = @(
    "run",
    "--project", $projectPath,
    "-c", "Release",
    "--no-launch-profile",
    "--"
)

if ($File) {
    $cliArgs += "-f"
    $cliArgs += $File
}

if ($Limit -gt 0) {
    $cliArgs += "-n"
    $cliArgs += $Limit.ToString()
}

if ($Lookahead -ne 2) {
    $cliArgs += "-l"
    $cliArgs += $Lookahead.ToString()
}

if ($Threads -gt 0) {
    $cliArgs += "-t"
    $cliArgs += $Threads.ToString()
}

if ($Csv) {
    $cliArgs += "-o"
    $cliArgs += $Csv
}

if ($SummaryOnly) {
    $cliArgs += "-s"
}

if ($VerboseOutput) {
    $cliArgs += "-v"
}

if ($Timeout -ne 30000) {
    $cliArgs += "--timeout"
    $cliArgs += $Timeout.ToString()
}

Write-Host "Running Sudoku Evaluator..." -ForegroundColor Cyan
& dotnet @cliArgs
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

