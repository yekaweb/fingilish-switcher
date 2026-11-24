# 06 — Phased Implementation Roadmap

## 📊 Project Dashboard

| Metric | Value |
|---|---|
| **Project** | `OmniSwitch Engine` (Universal Multilingual Keyboard Transducer) |
| **Stack** | C# .NET 8 / Win32 Interop / Inno Setup 6 / GitHub Actions |
| **Started** | 2026-11-24 |
| **Target Launch** | 2027-02-15 (12 Weeks) |
| **Overall Progress** | 15% (Core Fingilish Win32 engine verified) |
| **Phases** | 1 / 4 |
| **Tasks Completed** | 3 / 24 |
| **Estimated Total** | 160 Hours |
| **Active Blockers** | 0 |
| **Current Phase** | Phase 1: Dynamic Locale Discovery & Core Multilingual Matrix |
| **Current Sprint** | Sprint 1 |

---

## 🏁 Milestones

| Milestone | Target Date | Status | Description |
|-----------|:-----------:|:------:|-------------|
| **M1: Multi-Locale MVP** | Week 2–3 | 🟨 In Progress | Dynamic OS layout detection + Russian, Spanish, Persian, English verified in-place |
| **M2: Hangul & Asian Automaton** | Week 6–8 | ⬜ Not Started | Korean 2-Set composition engine + Trigram N-gram classifier + Community JSON layouts |
| **M3: Global Public Launch** | Week 10–12 | ⬜ Not Started | Production MSIX/Inno installer, winget distribution, dark slate luxury dashboard |

---

## 🗺️ Full Phased Roadmap

### Phase 1 — Dynamic Locale Discovery & Core Multilingual Matrix (Weeks 1–3)
**Goal:** Auto-detect all installed Windows keyboard layouts; support multi-way switching between English, Spanish, Russian, and Persian.

#### Sprint 1 (Weeks 1–2): Dynamic Win32 Locale Subsystem
- **Module: OS Input Enumeration**
  - Task: Implement `GetKeyboardLayoutList` wrapper in `NativeMethods.cs` `[P0]` `[6h]` 🟩
    - Subtask: P/Invoke declaration and struct mapping
    - Subtask: Unit test enumerating system KLIDs
  - Task: Listen to `WM_INPUTLANGCHANGE` via hidden message pump window `[P0]` `[8h]` 🟨
    - Subtask: Re-index layout map when user adds/removes Windows languages
    - Subtask: Auto-sync active layout in Tray context
  - Task: Multi-Way Cycle Engine `[P0]` `[10h]` ⬜
    - Subtask: Allow pressing `Ctrl + Shift + F` within 3 seconds to cycle through installed layouts (e.g. EN ➔ ES ➔ RU ➔ FA)
    - Subtask: Update floating bubble text to show target language flag/badge

#### Sprint 2 (Weeks 2–3): European Dead-Key & Spanish/German Layouts
- **Module: Diacritics & Accents**
  - Task: Spanish QWERTY mapping table (`ñ`, `á`, `é`, `í`, `ó`, `ú`, `¿`, `¡`) `[P0]` `[8h]` ⬜
  - Task: German QWERTZ mapping table (`ä`, `ö`, `ü`, `ß`, `Z` ↔ `Y` swap) `[P1]` `[6h]` ⬜
  - Task: Russian JCUKEN mapping table `[P0]` `[6h]` ⬜

---

### Phase 2 — Korean 2-Set Hangul Automaton & Micro-Classifier (Weeks 4–6)
**Goal:** In-place Korean syllable composition + sub-millisecond script detection.

#### Sprint 3 (Weeks 4–5): Hangul Syllable Automaton
- **Module: Hangul State Machine**
  - Task: Implement Chosung (19), Jungsung (21), Jongsung (28) math formula `[P0]` `[16h]` ⬜
    - Subtask: Map Latin letters to Jamo phonemes (`r`=ㄱ, `s`=ㄴ, `e`=ㄷ, `k`=ㅏ)
    - Subtask: Automaton state transitions for double consonants (`ㄳ`, `ㄵ`, `ㄶ`)
    - Subtask: Compound vowels (`ㅘ`, `ㅝ`, `ㅟ`)
  - Task: Unit test suite for Korean with 200 real-world words `[P0]` `[8h]` ⬜

#### Sprint 4 (Weeks 5–6): N-Gram Script Classifier
- **Module: Statistical Language Identification**
  - Task: Train 50KB static trigram frequency table across 12 languages `[P1]` `[12h]` ⬜
  - Task: Implement zero-allocation trigram rank scoring in C# `[P1]` `[8h]` ⬜
    - Subtask: Benchmark latency to guarantee <50 microseconds execution

---

### Phase 3 — Community Extensibility & Enterprise Packaging (Weeks 7–9)
**Goal:** Modular JSON layout repository + elevated admin helper + winget publication.

#### Sprint 5 (Weeks 7–8): Community JSON Layout Hub
- **Module: Schema & File Watcher**
  - Task: Define JSON schema for arbitrary keyboard layout mappings `[P1]` `[8h]` ⬜
  - Task: Implement `%AppData%/OmniSwitch/layouts/` file watcher `[P1]` `[6h]` ⬜
  - Task: Support Dvorak, Colemak, French AZERTY, Turkish Q/F community layouts `[P2]` `[8h]` ⬜

#### Sprint 6 (Weeks 8–9): Elevated Windows Companion (UIPI Bypass)
- **Module: Windows Security Integration**
  - Task: Admin Token detection on active window `[P1]` `[6h]` ⬜
  - Task: Optional Windows Scheduled Task with highest privileges for complete system coverage `[P1]` `[8h]` ⬜

---

### Phase 4 — Polishing, Performance & Global Launch (Weeks 10–12)
**Goal:** Luxury Taste-Skill UI, zero-leak validation, and international marketing.

#### Sprint 7 (Weeks 10–11): UI Luxury Overhaul
- **Module: Industrial Dashboard**
  - Task: Multi-language tab with live interactive layout keyboard visualizer `[P1]` `[12h]` ⬜
  - Task: Per-application blacklist (mute hooks in games / password managers) `[P0]` `[8h]` ⬜

#### Sprint 8 (Weeks 11–12): Distribution & Launch
- **Module: Release Automation**
  - Task: GitHub Actions CI/CD with automated Inno Setup 64-bit compilation `[P0]` `[6h]` ⬜
  - Task: Microsoft winget and Chocolatey package submission `[P0]` `[6h]` ⬜
  - Task: Hacker News & Product Hunt Launch Campaign `[P0]` `[8h]` ⬜

---

## 🚧 Blocker Log

| ID | Task | Blocker | Impact | Proposed Solution | Status | Opened |
|----|------|---------|--------|-------------------|:------:|:------:|
| — | — | None active | — | — | 🟩 Clear | — |

---

## ⚠️ Risk Register

| ID | Risk | Probability | Impact | Mitigation Strategy | Owner |
|----|------|:-----------:|:------:|---------------------|:-----:|
| **R1** | Windows Defender False Positive on global hooks | Medium | High | Sign binaries via SignPath.io; submit to MS Defender Whitelist | Agent |
| **R2** | Korean double-final consonant ambiguity (`넋` vs `너 + ㄱ + ㅅ`) | Medium | Medium | Implement standard Hangul backtracking automaton buffer | Agent |
| **R3** | Non-admin process unable to replace text in elevated windows | Medium | Medium | Provide optional elevated runner in installer | Agent |

---

## ⚡ Execute NOW (Next 60 Minutes)

- **Action:** Create `LocaleManager.cs` inside `src/FingilishSwitcher/` implementing `GetKeyboardLayoutList` and logging all active system keyboard layouts on application startup.
- **Why this first:** Proves dynamic OS input locale discovery without touching any existing Persian/English mapping logic.
- **Success signal:** Running the app logs the exact installed Windows language IDs (e.g., `0x0409` for English, `0x0429` for Persian, `0x0419` for Russian) in the diagnostic trace.
