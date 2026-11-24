# 04 — Risk Assessment & Pre-Mortem Failure Analysis

## 1. Risk Matrix

| Risk | Category | Probability (H/M/L) | Impact (H/M/L) | Mitigation Strategy |
|------|----------|:-------------------:|:--------------:|---------------------|
| **Antivirus False Positives** | Technical / Security | **Medium** | **High** | Global mouse/keyboard hooks trigger heuristic alerts in aggressive AVs. *Mitigation:* Apply for open-source code signing certificate via SignPath Foundation; maintain transparent reproducible GitHub Actions builds; submit clean binaries to Microsoft Defender Whitelist. |
| **Windows UIPI Elevation Barrier** | Technical | **Medium** | **Medium** | Standard user-level app cannot inject `Ctrl+V` into apps running as Administrator (e.g., Task Manager, Admin CMD). *Mitigation:* Offer an optional "Run as Administrator" task in Inno Setup; display subtle toast notification if active target window is elevated. |
| **Anti-Cheat Interference in Games** | Compatibility | **Low** | **High** | Ring-0 anti-cheat engines (Riot Vanguard, EasyAntiCheat) might flag low-level input hooks. *Mitigation:* Built-in "Gaming Auto-Pause" that automatically suspends mouse/keyboard hooks when full-screen exclusive DirectX/Vulkan games are detected. |
| **Complex IME De-sync (CJK)** | Technical | **Medium** | **Medium** | Languages with heavy IME logic (Japanese Kanji / Chinese Pinyin) require multi-stage dictionary candidate selection. *Mitigation:* Focus Phase 1 Korean on 2-Set Hangul (deterministic syllable math); leave Chinese/Japanese candidate menus to OS IME and focus on phonetic alphabets. |
| **Feature Bloat / Resource Inflation** | Execution | **Medium** | **High** | Adding too many AI models bloats memory from 15MB to 300MB. *Mitigation:* Strict architecture rule: Core engine remains <15MB RAM and 0.0% standby CPU; any AI or translation feature must run as a dynamic optional on-demand plugin. |

---

## 2. Pre-Mortem Analysis (18 Months From Now)

> *"It is 18 months from today, and OmniSwitch has failed. Nobody is using it, and the repository is archived. What killed it?"*

### Failure Mode #1: The False-Positive Malware Quarantine Trap
- **The Scenario:** An automated Windows Defender heuristic definition update flags the binary as `Win32/Trojan:Generic` due to the low-level mouse hook. Users freak out on Reddit and GitHub, accusing the project of being a keylogger, destroying the community's trust overnight.
- **Counter-Strategy:** 
  1. Automated VirusTotal scan badge embedded on every GitHub Release via GitHub Actions.
  2. Maintain a clear "Zero-Keystroke-Logging" security architecture whitepaper.
  3. Pre-emptively sign binaries with a verified open-source certificate.

### Failure Mode #2: The "Jack of All Trades, Master of None" Trap
- **The Scenario:** The team tries to simultaneously support 80 complex world languages, breaking Spanish dead-keys and Persian half-spaces with buggy edge cases. The tool feels unreliable, and users return to backspacing manually.
- **Counter-Strategy:**
  1. Tiered Language Rollout: Tier 1 (Latin, Cyrillic, Perso-Arabic, 2-Set Hangul) must have 100% test coverage before adding Tier 2 (Devanagari, Greek, Hebrew).
  2. Comprehensive automated unit test suite simulating thousands of real-world sentences across every supported layout.

### Failure Mode #3: The Background Clipboard Corruption Bug
- **The Scenario:** In a specific software (like Adobe Photoshop or Microsoft Excel), in-place text replacement corrupts rich formatted text or clears an important copied image in the user's clipboard.
- **Counter-Strategy:**
  1. Preserve previous clipboard format (restore previous non-text clipboard items if conversion fails).
  2. Implement a quick "Undo" (`Ctrl + Shift + F` twice) to restore original text instantly.

---

## 3. Red Lines & Stop Signals

1. **Memory Leak Stop Signal:** If resident memory exceeds **35 MB** during normal standby operation, all feature development halts until the leak is patched.
2. **Latency Stop Signal:** If text conversion turnaround takes longer than **120 milliseconds** from hotkey trigger to screen display, the architecture must be refactored.
3. **Security Stop Signal:** Under no circumstances will keystroke buffers ever be written to disk, sent over network sockets, or persisted beyond the immediate conversion transaction.
