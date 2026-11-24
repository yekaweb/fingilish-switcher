# Security Policy & Antivirus Verification 🛡️

## 1. Antivirus Scan Report (گزارش رسمی اسکن آنتی‌ویروس)

| Scanner | Version / Engine | Target | Status | Result |
|---|:---:|---|:---:|:---:|
| **Microsoft Defender Antivirus** | `MpCmdRun.exe` (Latest Definition) | `FingilishSwitcher_Setup_v1.5.0.exe` | 🟢 **CLEAN** | **0 Threats Found (Clean)** |
| **Microsoft Defender Antivirus** | `MpCmdRun.exe` (Latest Definition) | `FingilishSwitcher.exe` (Binary) | 🟢 **CLEAN** | **0 Threats Found (Clean)** |
| **VirusTotal Inspection** | 70+ AV Engines | `FingilishSwitcher_Setup_v1.5.0.exe` | 🟢 **CLEAN** | [View VirusTotal Report](https://www.virustotal.com/gui/file/35dc2ca2c766844023a5c5f34015f4cd23f6753684efa3809400007bb507bcc3) |

### Microsoft Defender Scan Verification Log:
```text
Scan starting...
Scan finished.
Scanning d:\Fingilish\installer\FingilishSwitcher_Setup_v1.5.0.exe found no threats.
Exit Code: 0 (STATUS_SUCCESS)
```

---

## 2. Cryptographic Checksums (هش‌های بررسی اصالت و سلامت فایل)

Before running the installer, you can verify its cryptographic hash on your machine to ensure it has not been modified or corrupted:

### `FingilishSwitcher_Setup_v1.5.0.exe` (Official Installer)
* **File Size:** `2,106,049 bytes` (~2.01 MB)
* **SHA-256:**
  ```text
  35DC2CA2C766844023A5C5F34015F4CD23F6753684EFA3809400007BB507BCC3
  ```
* **MD5:**
  ```text
  02A4767BB71BE6734FC702351F1ACE38
  ```

### `FingilishSwitcher.exe` (Core Executable)
* **SHA-256:**
  ```text
  05463C7462F5C9A454714C2E3E3252780C7AFA6481D605B7CFF7261D859CDFF8
  ```

### How to verify on Windows PowerShell:
```powershell
Get-FileHash .\FingilishSwitcher_Setup_v1.5.0.exe -Algorithm SHA256
```
Compare the output against the SHA-256 hash above.

---

## 3. Privacy & Zero-Telemetry Guarantee (تضمین ۱۰۰٪ حریم خصوصی)

Keyboard utilities can naturally raise questions about keystroke logging. **Fingilish Switcher** is engineered with strict industrial safety principles:

1. **🚫 Zero Keylogging (بدون لاگ کلیدها):**
   * The application registers a system hotkey (`Ctrl + Shift + F`) using `RegisterHotKey`.
   * It **NEVER** monitors, records, captures, or saves user typing to disk or memory.
2. **🔌 100% Offline (کاملاً آفلاین):**
   * Contains zero networking sockets, zero HTTP clients, and zero telemetry analytics.
   * Never connects to the internet or external servers.
3. **⚡ Clean Memory Handling:**
   * Text is read from the clipboard during the transformation in-place, and immediately discarded after replacement.
4. **📖 100% Open-Source:**
   * All code is fully open and audited on GitHub under the MIT License. Anyone can inspect and compile the code directly.

---

## 4. Microsoft SmartScreen & Defender Guidance

Because Fingilish Switcher is a newly released, community-driven open-source project without a costly ($400/yr) commercial EV code-signing certificate, Windows SmartScreen may show an informational message upon initial download (*"Windows protected your PC"*).

* Click **"More info" (اطلاعات بیشتر)** ➔ **"Run anyway" (اجرا در هر صورت)**.
* The installer is 100% safe, verified clean by Microsoft Defender, and free of any adware or bundleware.
* You can also submit the file to [Microsoft Security Intelligence](https://www.microsoft.com/en-us/wdsi/filesubmission) for independent automated analysis.

---

## 5. Reporting Security Vulnerabilities

If you discover any security issue or vulnerability, please report it responsibly by opening a GitHub issue or contacting the maintainers at `support@yekaweb.com`. We respond within 24 hours.
