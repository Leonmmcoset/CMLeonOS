$buildTime = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$buildTime | Out-File -FilePath "BuildTime.txt" -Encoding ASCII -NoNewline
Write-Host "Build time written to BuildTime.txt: $buildTime"
