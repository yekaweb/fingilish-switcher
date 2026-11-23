@echo off
chcp 65001 > nul
title نصب Fingilish Switcher
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Install.ps1"
pause
