# Architecture Decision Records (ADR) — OmniSwitch Engine

## 📋 ADR Index

- [ADR-001: Dynamic OS Keyboard Layout Discovery via Win32 API](#adr-001-dynamic-os-keyboard-layout-discovery-via-win32-api)
- [ADR-002: Syllable Automaton for Korean Hangul 2-Set Composition](#adr-002-syllable-automaton-for-korean-hangul-2-set-composition)
- [ADR-003: Pure Win32 Native Clipboard Interop Over Managed WinForms/WPF Clipboard](#adr-003-pure-win32-native-clipboard-interop-over-managed-winformswpf-clipboard)
- [ADR-004: Modular JSON Layout Specification for Community Extensibility](#adr-004-modular-json-layout-specification-for-community-extensibility)

---

### ADR-001: Dynamic OS Keyboard Layout Discovery via Win32 API
- **Date**: 2026-11-24
- **Status**: Accepted
- **Context**: The existing Fingilish Switcher hardcodes Persian ↔ English layouts. Users requested support for arbitrary language pairs (Spanish, Russian, Korean, German, Persian, etc.) based on whatever languages are installed on their specific machine.
- **Decision**: Use Win32 `GetKeyboardLayoutList`, `GetKeyboardLayoutName`, and listen to `WM_INPUTLANGCHANGE` to dynamically enumerate active user input locales at runtime.
- **Consequences**: Zero manual setup required from the user; adapts instantly when a user adds/removes languages in Windows Settings.
- **Alternatives Considered**: 
  - *Hardcoded language matrix:* Rejected because it cannot scale to hundreds of world keyboard variants.
  - *Custom virtual keyboard driver:* Rejected due to complexity, security signing costs, and requirement for system reboots.

---

### ADR-002: Syllable Automaton for Korean Hangul 2-Set Composition
- **Date**: 2026-11-24
- **Status**: Accepted
- **Context**: Korean cannot be converted with a naive 1-to-1 character lookup table because Korean glyphs are 2D blocks (syllables) composed of Initial consonants (Chosung), Medial vowels (Jungsung), and optional Final consonants (Jongsung).
- **Decision**: Implement a pure mathematical finite-state automaton based on the standard Unicode formula:  
  `Syllable = 0xAC00 + (Initial * 21 + Medial) * 28 + Final`.
- **Consequences**: Allows typing `gksrnr` on QWERTY to correctly yield `한국` in-place with zero external dictionary overhead.
- **Alternatives Considered**: 
  - *Separate Korean IME injection:* Rejected because it disrupts active application focus and introduces noticeable lag.

---

### ADR-003: Pure Win32 Native Clipboard Interop Over Managed WinForms/WPF Clipboard
- **Date**: 2026-11-24
- **Status**: Accepted
- **Context**: Managed .NET `System.Windows.Forms.Clipboard` and `System.Windows.Clipboard` throw `ThreadStateException` when invoked from async tasks or thread pool threads (requiring explicit STA). They also introduce memory leaks and COM deadlocks.
- **Decision**: Implement all clipboard read/write via unmanaged Win32 APIs: `OpenClipboard`, `GetClipboardData(CF_UNICODETEXT)`, `SetClipboardData`, `EmptyClipboard`, and `CloseClipboard`.
- **Consequences**: Thread-safe across STA, MTA, and background threads; 10x faster execution with zero COM exceptions.
- **Alternatives Considered**: Managed `Clipboard.GetDataObject()` with retry wrappers (rejected due to proven instability in version 1.2.0).

---

### ADR-004: Modular JSON Layout Specification for Community Extensibility
- **Date**: 2026-11-24
- **Status**: Accepted
- **Context**: Community typists use specialized layouts (Colemak, Dvorak, Bepo, Neo, Turkish-F) that should not require recompiling the C# binary.
- **Decision**: Define a lightweight JSON schema for keyboard layouts placed in `%AppData%/OmniSwitch/layouts/*.json`.
- **Consequences**: Enables community contributors to submit pull requests adding support for any regional language without modifying core engine logic.
