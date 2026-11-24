using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FingilishSwitcher
{
    public enum ScriptType
    {
        Latin,
        PersianArabic,
        Cyrillic,
        Korean,
        Unknown
    }

    public static class MultiKeyboardMapper
    {
        // -------------------------------------------------------------
        // Russian (JCUKEN ЙЦУКЕН) Mappings
        // -------------------------------------------------------------
        private static readonly Dictionary<char, char> EnToRu = new()
        {
            // Lowercase
            {'q', 'й'}, {'w', 'ц'}, {'e', 'у'}, {'r', 'к'}, {'t', 'е'},
            {'y', 'н'}, {'u', 'г'}, {'i', 'ш'}, {'o', 'щ'}, {'p', 'з'},
            {'[', 'х'}, {']', 'ъ'},
            {'a', 'ф'}, {'s', 'ы'}, {'d', 'в'}, {'f', 'а'}, {'g', 'п'},
            {'h', 'р'}, {'j', 'о'}, {'k', 'л'}, {'l', 'д'}, {';', 'ж'},
            {'\'', 'э'},
            {'z', 'я'}, {'x', 'ч'}, {'c', 'с'}, {'v', 'м'}, {'b', 'и'},
            {'n', 'т'}, {'m', 'ь'}, {',', 'б'}, {'.', 'ю'}, {'/', '.'},
            {'`', 'ё'},

            // Uppercase
            {'Q', 'Й'}, {'W', 'Ц'}, {'E', 'У'}, {'R', 'К'}, {'T', 'Е'},
            {'Y', 'Н'}, {'U', 'Г'}, {'I', 'Ш'}, {'O', 'Щ'}, {'P', 'З'},
            {'{', 'Х'}, {'}', 'Ъ'},
            {'A', 'Ф'}, {'S', 'Ы'}, {'D', 'В'}, {'F', 'А'}, {'G', 'П'},
            {'H', 'Р'}, {'J', 'О'}, {'K', 'Л'}, {'L', 'Д'}, {':', 'Ж'},
            {'"', 'Э'},
            {'Z', 'Я'}, {'X', 'Ч'}, {'C', 'С'}, {'V', 'М'}, {'B', 'И'},
            {'N', 'Т'}, {'M', 'Ь'}, {'<', 'Б'}, {'>', 'Ю'}, {'?', ','},
            {'~', 'Ё'},

            // Shifted Numbers / Punctuation
            {'@', '"'}, {'#', '№'}, {'$', ';'}, {'^', ':'}, {'&', '?'}
        };

        private static readonly Dictionary<char, char> RuToEn = new();

        // -------------------------------------------------------------
        // Spanish (QWERTY with ñ, acute accents, ¿, ¡) Mappings
        // -------------------------------------------------------------
        private static readonly Dictionary<char, char> EnToEs = new()
        {
            {';', 'ñ'}, {':', 'Ñ'},
            {'\'', '´'}, {'"', '¨'},
            {'[', '`'}, {'{', '^'},
            {']', '+'}, {'}', '*'},
            {'=', '¿'}, {'+', '¡'},
            {'\\', 'ç'}, {'|', 'Ç'},
            {'<', '<'}, {'>', '>'}
        };

        private static readonly Dictionary<char, char> EsToEn = new();

        static MultiKeyboardMapper()
        {
            // Build Ru -> En reverse lookup
            foreach (var kvp in EnToRu)
            {
                RuToEn[kvp.Value] = kvp.Key;
            }

            // Build Es -> En reverse lookup
            foreach (var kvp in EnToEs)
            {
                EsToEn[kvp.Value] = kvp.Key;
            }
            EsToEn['ñ'] = ';';
            EsToEn['Ñ'] = ':';
            EsToEn['¿'] = '=';
            EsToEn['¡'] = '+';
            EsToEn['´'] = '\'';
            EsToEn['ç'] = '\\';
            EsToEn['Ç'] = '|';
        }

        public static ScriptType DetectScript(string text)
        {
            if (string.IsNullOrEmpty(text))
                return ScriptType.Unknown;

            int latin = 0;
            int persian = 0;
            int cyrillic = 0;
            int korean = 0;

            foreach (char c in text)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    latin++;
                else if (IsPersianArabic(c))
                    persian++;
                else if (IsCyrillic(c))
                    cyrillic++;
                else if (IsKorean(c))
                    korean++;
            }

            if (persian >= latin && persian >= cyrillic && persian >= korean && persian > 0)
                return ScriptType.PersianArabic;
            if (cyrillic >= latin && cyrillic >= persian && cyrillic >= korean && cyrillic > 0)
                return ScriptType.Cyrillic;
            if (korean >= latin && korean >= persian && korean >= cyrillic && korean > 0)
                return ScriptType.Korean;
            if (latin > 0)
                return ScriptType.Latin;

            return ScriptType.Unknown;
        }

        public static bool IsPersianArabic(char c)
        {
            return (c >= '\u0600' && c <= '\u06FF') ||
                   (c >= '\uFB50' && c <= '\uFDFF') ||
                   (c >= '\uFE70' && c <= '\uFEFF') ||
                   (c >= '۰' && c <= '۹') ||
                   c == '؟' || c == '،' || c == '‌';
        }

        public static bool IsCyrillic(char c)
        {
            return (c >= '\u0400' && c <= '\u04FF') || (c >= '\u0500' && c <= '\u052F');
        }

        public static bool IsKorean(char c)
        {
            return (c >= '\uAC00' && c <= '\uD7AF') || (c >= '\u1100' && c <= '\u11FF') || (c >= '\u3130' && c <= '\u318F');
        }

        public static string EnglishToRussian(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                sb.Append(EnToRu.TryGetValue(c, out char r) ? r : c);
            }
            return sb.ToString();
        }

        public static string RussianToEnglish(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                sb.Append(RuToEn.TryGetValue(c, out char e) ? e : c);
            }
            return sb.ToString();
        }

        public static string EnglishToSpanish(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                sb.Append(EnToEs.TryGetValue(c, out char s) ? s : c);
            }
            return sb.ToString();
        }

        public static string SpanishToEnglish(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                sb.Append(EsToEn.TryGetValue(c, out char e) ? e : c);
            }
            return sb.ToString();
        }

        public static string EnglishToKorean(string text) => HangulAutomaton.Compose(text);
        public static string KoreanToEnglish(string text) => HangulAutomaton.Decompose(text);

        /// <summary>
        /// Converts the given text to English Latin from any supported non-Latin script.
        /// </summary>
        public static string ToEnglish(string text, KeyboardLayoutType faLayout = KeyboardLayoutType.MicrosoftPersian)
        {
            var script = DetectScript(text);
            return script switch
            {
                ScriptType.PersianArabic => KeyboardMapper.PersianToEnglish(text, faLayout),
                ScriptType.Cyrillic => RussianToEnglish(text),
                ScriptType.Korean => HangulAutomaton.Decompose(text),
                ScriptType.Latin => SpanishToEnglish(text),
                _ => text
            };
        }

        /// <summary>
        /// Converts an English Latin string into the specified target language code.
        /// </summary>
        public static string FromEnglish(string text, string targetCode, KeyboardLayoutType faLayout = KeyboardLayoutType.MicrosoftPersian)
        {
            return targetCode.ToLowerInvariant() switch
            {
                "fa" or "per" or "fas" => KeyboardMapper.EnglishToPersian(text, faLayout),
                "ru" or "rus" => EnglishToRussian(text),
                "es" or "spa" => EnglishToSpanish(text),
                "ko" or "kor" => HangulAutomaton.Compose(text),
                _ => text
            };
        }

        /// <summary>
        /// High-level universal convert: converts between the detected current language and target language.
        /// </summary>
        public static string Convert(string text, string targetCode, KeyboardLayoutType faLayout = KeyboardLayoutType.MicrosoftPersian)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var currentScript = DetectScript(text);

            // If target is English
            if (targetCode.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                return ToEnglish(text, faLayout);
            }

            // If current is already English/Latin, convert directly to target
            if (currentScript == ScriptType.Latin)
            {
                return FromEnglish(text, targetCode, faLayout);
            }

            // Cross-script conversion (e.g. Russian -> Persian, or Persian -> Russian):
            // We first normalize to English QWERTY intermediate scancodes, then to target!
            string intermediateEn = ToEnglish(text, faLayout);
            return FromEnglish(intermediateEn, targetCode, faLayout);
        }

        /// <summary>
        /// Smart Cycle Morphing: Automatically detects current script and cycles to the next installed OS language.
        /// </summary>
        public static string CycleConvert(
            string text,
            IReadOnlyList<InstalledLocale> activeLocales,
            ref string lastTargetCode,
            out string nextTargetBadge,
            KeyboardLayoutType faLayout = KeyboardLayoutType.MicrosoftPersian)
        {
            if (string.IsNullOrEmpty(text))
            {
                nextTargetBadge = "EN";
                return text;
            }

            // Ensure we have at least English and Persian
            var locales = (activeLocales != null && activeLocales.Count > 0)
                ? activeLocales.ToList()
                : new List<InstalledLocale>
                {
                    LocaleManager.CreateLocaleFromLcid(IntPtr.Zero, 0x0409),
                    LocaleManager.CreateLocaleFromLcid(IntPtr.Zero, 0x0429)
                };

            // Detect current script
            var script = DetectScript(text);
            string currentCode = script switch
            {
                ScriptType.PersianArabic => "fa",
                ScriptType.Cyrillic => "ru",
                ScriptType.Korean => "ko",
                _ => "en"
            };

            // If lastTargetCode was set (i.e. user is cycling rapidly), find next index after lastTargetCode
            string lookupCode = lastTargetCode ?? currentCode;
            int currentIndex = locales.FindIndex(x => x.Code.Equals(lookupCode, StringComparison.OrdinalIgnoreCase));
            if (currentIndex == -1)
            {
                currentIndex = locales.FindIndex(x => x.Code.Equals(currentCode, StringComparison.OrdinalIgnoreCase));
            }

            int nextIndex = (currentIndex + 1) % locales.Count;
            
            // If only 1 locale exists, toggle between EN and FA
            if (locales.Count == 1)
            {
                string fallbackTarget = currentCode == "fa" ? "en" : "fa";
                lastTargetCode = fallbackTarget;
                nextTargetBadge = fallbackTarget.ToUpperInvariant();
                return Convert(text, fallbackTarget, faLayout);
            }

            var nextLocale = locales[nextIndex];
            lastTargetCode = nextLocale.Code;
            nextTargetBadge = nextLocale.Badge;

            return Convert(text, nextLocale.Code, faLayout);
        }
    }
}
