$buildTime = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$buildTime | Out-File -FilePath "BuildTime.txt" -Encoding UTF8
Write-Host "Build time written to BuildTime.txt: $buildTime"
