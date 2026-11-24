using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FingilishSwitcher
{
    public record InstalledLocale
    {
        public IntPtr Hkl { get; init; }
        public int Lcid { get; init; }
        public string Code { get; init; } = "en";
        public string EnglishName { get; init; } = "English";
        public string NativeName { get; init; } = "English";
        public string Badge { get; init; } = "EN";
        public string Flag { get; init; } = "🌐";
    }

    public static class LocaleManager
    {
        private static readonly object _syncRoot = new();
        private static List<InstalledLocale> _cachedLocales = new();
        public static event Action? OnLocalesChanged;

        static LocaleManager()
        {
            RefreshLocales();
        }

        public static IReadOnlyList<InstalledLocale> GetInstalledLocales()
        {
            lock (_syncRoot)
            {
                if (_cachedLocales.Count == 0)
                {
                    RefreshLocalesInternal();
                }
                return _cachedLocales.ToList();
            }
        }

        public static void RefreshLocales()
        {
            lock (_syncRoot)
            {
                RefreshLocalesInternal();
            }
            OnLocalesChanged?.Invoke();
        }

        private static void RefreshLocalesInternal()
        {
            var result = new List<InstalledLocale>();
            try
            {
                int count = NativeMethods.GetKeyboardLayoutList(0, null);
                if (count > 0)
                {
                    IntPtr[] hkls = new IntPtr[count];
                    int actualCount = NativeMethods.GetKeyboardLayoutList(count, hkls);

                    for (int i = 0; i < actualCount; i++)
                    {
                        IntPtr hkl = hkls[i];
                        int lcid = (int)((long)hkl & 0xFFFF);
                        var locale = CreateLocaleFromLcid(hkl, lcid);
                        if (!result.Any(x => x.Code == locale.Code))
                        {
                            result.Add(locale);
                        }
                    }
                }
            }
            catch
            {
                // Fallback to English and Persian if Win32 API enumeration fails
            }

            if (result.Count == 0)
            {
                result.Add(CreateLocaleFromLcid(IntPtr.Zero, 0x0409)); // EN
                result.Add(CreateLocaleFromLcid(IntPtr.Zero, 0x0429)); // FA
            }

            _cachedLocales = result;
        }

        public static InstalledLocale? GetActiveForegroundLocale()
        {
            try
            {
                IntPtr hwnd = NativeMethods.GetForegroundWindow();
                if (hwnd != IntPtr.Zero)
                {
                    uint threadId = NativeMethods.GetWindowThreadProcessId(hwnd, out _);
                    IntPtr hkl = NativeMethods.GetKeyboardLayout(threadId);
                    int lcid = (int)((long)hkl & 0xFFFF);
                    var list = GetInstalledLocales();
                    return list.FirstOrDefault(x => x.Lcid == lcid) ?? CreateLocaleFromLcid(hkl, lcid);
                }
            }
            catch
            {
            }
            return null;
        }

        public static InstalledLocale CreateLocaleFromLcid(IntPtr hkl, int lcid)
        {
            string code = "en";
            string engName = "English";
            string nativeName = "English";
            string badge = "EN";
            string flag = "🇺🇸";

            try
            {
                var ci = CultureInfo.GetCultureInfo(lcid);
                code = ci.TwoLetterISOLanguageName.ToLowerInvariant();
                engName = ci.EnglishName;
                nativeName = ci.NativeName;
                badge = code.ToUpperInvariant();
            }
            catch
            {
            }

            // Assign badges and flags for common polyglot languages
            switch (lcid & 0xFF)
            {
                case 0x29: // Persian / Farsi
                    code = "fa";
                    badge = "FA";
                    flag = "🇮🇷";
                    if (string.IsNullOrEmpty(nativeName) || nativeName == "English")
                    {
                        engName = "Persian";
                        nativeName = "فارسی";
                    }
                    break;

                case 0x09: // English
                    code = "en";
                    badge = "EN";
                    flag = "🇺🇸";
                    break;

                case 0x19: // Russian
                    code = "ru";
                    badge = "RU";
                    flag = "🇷🇺";
                    if (string.IsNullOrEmpty(nativeName) || nativeName == "English")
                    {
                        engName = "Russian";
                        nativeName = "Русский";
                    }
                    break;

                case 0x0A: // Spanish
                    code = "es";
                    badge = "ES";
                    flag = "🇪🇸";
                    if (string.IsNullOrEmpty(nativeName) || nativeName == "English")
                    {
                        engName = "Spanish";
                        nativeName = "Español";
                    }
                    break;

                case 0x12: // Korean
                    code = "ko";
                    badge = "KO";
                    flag = "🇰🇷";
                    if (string.IsNullOrEmpty(nativeName) || nativeName == "English")
                    {
                        engName = "Korean";
                        nativeName = "한국어";
                    }
                    break;

                case 0x07: // German
                    code = "de";
                    badge = "DE";
                    flag = "🇩🇪";
                    break;

                case 0x0C: // French
                    code = "fr";
                    badge = "FR";
                    flag = "🇫🇷";
                    break;

                case 0x01: // Arabic
                    code = "ar";
                    badge = "AR";
                    flag = "🇸🇦";
                    break;

                case 0x1F: // Turkish
                    code = "tr";
                    badge = "TR";
                    flag = "🇹🇷";
                    break;
            }

            return new InstalledLocale
            {
                Hkl = hkl,
                Lcid = lcid,
                Code = code,
                EnglishName = engName,
                NativeName = nativeName,
                Badge = badge,
                Flag = flag
            };
        }
    }
}
