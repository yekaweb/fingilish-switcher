# Fingilish Switcher Automated Windows Installer v1.1.0
$ErrorActionPreference = "Stop"

$installDir = Join-Path $env:LOCALAPPDATA "Programs\FingilishSwitcher"
$sourceDir = Join-Path $PSScriptRoot "dist"

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Fingilish Switcher Industrial Installer v1.1.0  " -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# 1. Stop any running instance
Write-Host "[1/5] Stopping any running instance..." -ForegroundColor Yellow
Get-Process -Name "FingilishSwitcher" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 500

# 2. Copy files to destination
Write-Host "[2/5] Installing application files to: $installDir" -ForegroundColor Yellow
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
}

Copy-Item -Path "$sourceDir\*" -Destination $installDir -Recurse -Force

# 3. Create Start Menu Shortcut
Write-Host "[3/5] Creating Start Menu shortcut..." -ForegroundColor Yellow
$startMenuDir = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs"
$shortcutPath = Join-Path $startMenuDir "Fingilish Switcher.lnk"
$exePath = Join-Path $installDir "FingilishSwitcher.exe"

$wscript = New-Object -ComObject WScript.Shell
$shortcut = $wscript.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $exePath
$shortcut.WorkingDirectory = $installDir
$shortcut.Description = "سامانه صنعتی تبدیل هوشمند دوطرفه متن‌های اشتباه تایپ‌شده بین فارسی و انگلیسی"
$shortcut.IconLocation = "$exePath,0"
$shortcut.Save()

# 4. Set Run with Windows in Registry
Write-Host "[4/5] Enabling Windows Startup entry..." -ForegroundColor Yellow
$regKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Set-ItemProperty -Path $regKey -Name "FingilishSwitcher" -Value "`"$exePath`""

# 5. Launch Application
Write-Host "[5/5] Launching Fingilish Switcher Industrial Edition..." -ForegroundColor Yellow
Start-Process -FilePath $exePath -WorkingDirectory $installDir

Write-Host ""
Write-Host "==================================================" -ForegroundColor Green
Write-Host "    نصب نسخه ۱.۱.۰ با موفقیت کامل انجام شد! ✔    " -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green
Write-Host ""
Write-Host "• پیشخوان صنعتی برنامه (Industrial Dashboard) باز شد." -ForegroundColor White
Write-Host "• فونت اصیل وزیر متن (Vazirmatn) فعال گردید." -ForegroundColor White
Write-Host "• مشکل دابل‌کلیک پوشه‌ها و دسکتاپ به طور کامل رفع شد." -ForegroundColor White
Write-Host "• تبدیل در تمام نرم‌افزارهای خارجی (کروم، ورد، تلگرام و...) فعال است." -ForegroundColor White
Write-Host ""
