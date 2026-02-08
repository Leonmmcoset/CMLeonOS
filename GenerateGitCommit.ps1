$ErrorActionPreference = "Stop"

$gitPath = "git"
$commitFile = "GitCommit.txt"

try {
    $commitHash = git rev-parse HEAD 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        $shortHash = $commitHash.Substring(0, 7).Trim()
        $trimmedHash = $commitHash.Trim()
        [System.IO.File]::WriteAllText($commitFile, $shortHash, [System.Text.Encoding]::ASCII)
        Write-Host "Git Commit Hash: $shortHash" -ForegroundColor Green
        Write-Host "Full Hash: $trimmedHash" -ForegroundColor Cyan
    }
    else {
        Write-Host "Warning: Not a git repository or git not found" -ForegroundColor Yellow
        Write-Host "Using default commit hash: unknown" -ForegroundColor Yellow
        "unknown" | Out-File -FilePath $commitFile -Encoding UTF8
    }
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    "unknown" | Out-File -FilePath $commitFile -Encoding UTF8
}