# OmniSwitch Engine — Executive Ideation Masterplan

## 💡 The Bet
> **We bet that in a globalized remote economy, the keyboard layout should be dynamic, invisible, and self-correcting—and that millions of multilingual users will choose an open-source, zero-telemetry, ultra-fast utility over abandoned, ad-ridden legacy tools.**

---

## 🛑 The Problem (Brutal Honesty)
Operating systems treat human languages as isolated binary switches (`Alt + Shift`). When typists forget to switch layouts, their thoughts turn into unreadable gibberish across chat, code, and search bars. 
Existing tools are either:
1. **Abandoned or untrusted** (Yandex's Punto Switcher bundled with telemetry and banned by corporate security).
2. **Strictly parochial** (only Russian/English or Hebrew/English, ignoring Spanish, Persian, Arabic, and Korean).
3. **Clunky and bloated** (100MB+ Qt frameworks that hijack clipboards and cause typing lag).

---

## ⚡ The Solution
**OmniSwitch Engine** is a universal, sub-millisecond keyboard layout transducer for Windows that:
1. **Dynamically auto-discovers** whatever keyboard languages the user has installed in Windows.
2. **Instantly morphs mistyped text in-place** across arbitrary language pairs with a single hotkey (`Ctrl + Shift + F`) or non-stealing floating bubble (`⇄`).
3. **Supports complex Asian scripts** via an embedded 2-Set Hangul Syllable Automaton that correctly fuses consonants and vowels into clean Unicode blocks (`gksrnr` ➔ `한국`).
4. **Guarantees 100% offline privacy** with zero keystroke logging, zero telemetry, <15 MB RAM, and 0.0% standby CPU.

---

## 🏆 Why This Wins (3 Unfair Advantages)

1. **Battle-Tested Native Win32 Subsystem (From Fingilish Switcher):**  
   While competitors struggle with managed clipboard race conditions and STA threading deadlocks, our unmanaged Win32 Native Clipboard and physical modifier synchronization engine replaces text reliably across Google Chrome, Word, VS Code, and Telegram with zero lag.
2. **Pure Mathematical Syllable Engine (Korean 2-Set Automaton):**  
   We solve Korean input in-place using standard Unicode syllable math (`0xAC00 + (I*21 + M)*28 + F`), giving us an uncontested monopoly in the massive Korean-English bilingual market.
3. **Radical Trust & Zero-Telemetry Moat:**  
   In an era where proprietary tools are feared as potential keyloggers, our 100% open-source GitHub codebase with reproducible builds provides unbeatable institutional and developer trust.

---

## 📈 Market Snapshot
- **TAM:** 1.4 Billion multilingual computer users worldwide.
- **SAM:** 180 Million daily bilingual typists switching layouts 50+ times per day.
- **Monetization:** 100% Free Open-Source Core (MIT) + Optional Pro Team Glossary Sync ($29 one-time).
- **GTM Channel #1:** Hacker News "Show HN" + Package Managers (`winget install OmniSwitch`).

---

## 🎯 The Number That Matters
> **100,000 Active Monthly Developers & Polyglots within 12 months, saving an estimated 4.2 million keystrokes daily.**

---

## ⚠️ Top Risk & Counter-Strategy
- **Risk:** Windows Defender false-positive alert on low-level mouse/keyboard hooks.
- **Counter-Strategy:** Verified open-source code signing via SignPath.io, pre-release Microsoft Defender Whitelist submission, and automated VirusTotal badges on all releases.

---

## 📊 Scoring: 50 / 60 — Tier 1 Breakthrough Project

| Technical | Impact | Speed to MVP | Moat | AI Leverage | Founder Fit | Total |
|:---------:|:------:|:------------:|:----:|:-----------:|:-----------:|:-----:|
| 8/10 | 9/10 | 9/10 | 8/10 | 7/10 | 9/10 | **50/60** |

---

## 🗺️ Roadmap at a Glance

| Phase | Goal | Duration | Key Deliverable |
|-------|------|:--------:|-----------------|
| **Phase 1: Dynamic Locale Matrix** | Auto-detect OS layouts + Spanish, Russian, Persian | Weeks 1–3 | Multi-way cycle morphing on Windows |
| **Phase 2: Asian Automata & N-Gram** | Korean 2-Set syllable engine + Trigram classifier | Weeks 4–6 | In-place Hangul composition (`한국`) |
| **Phase 3: Community JSON Layouts** | Support custom layouts (Dvorak, Colemak, etc.) | Weeks 7–9 | Modular layout loading from AppData |
| **Phase 4: Global Launch** | Taste-Skill UI, winget package, global marketing | Weeks 10–12 | Public release on GitHub, winget, HN |

---

## ⚡ Execute NOW (Next 60 Minutes)
Implement `LocaleManager.cs` to invoke Win32 `GetKeyboardLayoutList`, printing the user's active keyboard layouts to prove dynamic multi-locale discovery.
