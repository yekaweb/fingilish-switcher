# Fingilish Switcher Automated Windows Uninstaller
$ErrorActionPreference = "SilentlyContinue"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "    Fingilish Switcher Windows Uninstaller        " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# 1. Stop Process
Write-Host "[1/4] Stopping running process..." -ForegroundColor Yellow
Get-Process -Name "FingilishSwitcher" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 500

# 2. Remove Registry Run Key
Write-Host "[2/4] Removing Windows Startup entry..." -ForegroundColor Yellow
$regKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Remove-ItemProperty -Path $regKey -Name "FingilishSwitcher" -ErrorAction SilentlyContinue

# 3. Remove Shortcut
Write-Host "[3/4] Removing Start Menu shortcut..." -ForegroundColor Yellow
$shortcutPath = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs\Fingilish Switcher.lnk"
if (Test-Path $shortcutPath) {
    Remove-Item -Path $shortcutPath -Force
}

# 4. Remove Files
Write-Host "[4/4] Removing program files..." -ForegroundColor Yellow
$installDir = Join-Path $env:LOCALAPPDATA "Programs\FingilishSwitcher"
if (Test-Path $installDir) {
    Remove-Item -Path $installDir -Recurse -Force
}

# 5. Remove Config (Optional, leave clean)
$configDir = Join-Path $env:APPDATA "FingilishSwitcher"
if (Test-Path $configDir) {
    Remove-Item -Path $configDir -Recurse -Force
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Green
Write-Host "    برنامه با موفقیت کامل از سیستم حذف گردید. ✔  " -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host ""
