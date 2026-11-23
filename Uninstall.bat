@echo off
chcp 65001 > nul
title حذف Fingilish Switcher
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Uninstall.ps1"
pause
