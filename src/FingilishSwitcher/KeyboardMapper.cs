using System;
using System.Collections.Generic;
using System.Text;

namespace FingilishSwitcher
{
    public enum KeyboardLayoutType
    {
        MicrosoftPersian, // Default Windows Persian layout (m -> ئ, \ -> پ)
        ISIRI9147         // Persian Standard layout (m -> پ, \ -> ژ)
    }

    public static class KeyboardMapper
    {
        // En -> Fa (Unshifted)
        private static readonly Dictionary<char, char> EnToFaNormal = new()
        {
            {'q', 'ض'}, {'w', 'ص'}, {'e', 'ث'}, {'r', 'ق'}, {'t', 'ف'},
            {'y', 'غ'}, {'u', 'ع'}, {'i', 'ه'}, {'o', 'خ'}, {'p', 'ح'},
            {'[', 'ج'}, {']', 'چ'}, {'\\', 'پ'},
            {'a', 'ش'}, {'s', 'س'}, {'d', 'ی'}, {'f', 'ب'}, {'g', 'ل'},
            {'h', 'ا'}, {'j', 'ت'}, {'k', 'ن'}, {'l', 'م'}, {';', 'ک'},
            {'\'', 'گ'},
            {'z', 'ظ'}, {'x', 'ط'}, {'c', 'ز'}, {'v', 'ر'}, {'b', 'ذ'},
            {'n', 'د'}, {'m', 'ئ'}, // In MS Persian: m -> ئ
            {',', 'و'}, {'.', '.'}, {'/', '/'}, {'`', '‌'}, // ZWNJ on `
            {'0', '۰'}, {'1', '۱'}, {'2', '۲'}, {'3', '۳'}, {'4', '۴'},
            {'5', '۵'}, {'6', '۶'}, {'7', '۷'}, {'8', '۸'}, {'9', '۹'}
        };

        // En -> Fa (Shifted)
        private static readonly Dictionary<char, char> EnToFaShifted = new()
        {
            {'Q', 'ْ'}, {'W', 'ٌ'}, {'E', 'ٍ'}, {'R', 'ً'}, {'T', 'ُ'},
            {'Y', 'ِ'}, {'U', 'َ'}, {'I', 'ّ'}, {'O', ']'}, {'P', '['},
            {'{', '}'}, {'}', '{'}, {'|', '|'},
            {'A', 'ؤ'}, {'S', 'ئ'}, {'D', 'ي'}, {'F', 'إ'}, {'G', 'أ'},
            {'H', 'آ'}, {'J', 'ة'}, {'K', '»'}, {'L', '«'}, {':', ':'},
            {'"', '"'},
            {'Z', 'ك'}, {'X', 'ٓ'}, {'C', 'ژ'}, {'V', 'ٰ'}, {'B', '‌'}, // ZWNJ
            {'N', 'ٔ'}, {'M', 'ء'},
            {'<', '>'}, {'>', '<'}, {'?', '؟'}, {'~', '÷'},
            {'!', '!'}, {'@', '،'}, {'#', '#'}, {'$', '﷼'}, {'%', '٪'},
            {'^', '×'}, {'&', '&'}, {'*', '*'}, {'(', ')'}, {')', '('},
            {'_', '_'}, {'+', '+'}
        };

        // ISIRI 9147 overrides
        private static readonly Dictionary<char, char> IsiriOverrides = new()
        {
            {'m', 'پ'},
            {'M', 'ئ'},
            {'\\', 'ژ'},
            {'|', '|'}
        };

        // Reverse lookup dictionaries: Fa -> En
        private static readonly Dictionary<char, char> FaToEn = new();
        private static readonly Dictionary<char, char> FaToEnIsiri = new();

        static KeyboardMapper()
        {
            // Build base reverse mappings from Normal
            foreach (var kvp in EnToFaNormal)
            {
                FaToEn[kvp.Value] = kvp.Key;
            }

            // Reverse digits explicitly
            for (char c = '0'; c <= '9'; c++)
            {
                char faDigit = (char)('۰' + (c - '0'));
                FaToEn[faDigit] = c;
            }

            // Build reverse mappings from Shifted (these take priority for characters only on Shift like آ, ژ, etc.)
            foreach (var kvp in EnToFaShifted)
            {
                FaToEn[kvp.Value] = kvp.Key;
            }

            // Ensure specific character mappings are exact
            FaToEn['آ'] = 'H'; // Shift+h
            FaToEn['ژ'] = 'C'; // Shift+c
            FaToEn['‌'] = 'B'; // Shift+b (ZWNJ)
            FaToEn['ء'] = 'M'; // Shift+m
            FaToEn['أ'] = 'G'; // Shift+g
            FaToEn['إ'] = 'F'; // Shift+f
            FaToEn['ؤ'] = 'A'; // Shift+a
            FaToEn['ة'] = 'J'; // Shift+j
            FaToEn['ي'] = 'D'; // Shift+d
            FaToEn['ك'] = 'Z'; // Shift+z
            FaToEn['»'] = 'K'; // Shift+k
            FaToEn['«'] = 'L'; // Shift+l
            FaToEn['؟'] = '?'; // Shift+/
            FaToEn['،'] = '@'; // Shift+2
            FaToEn['٪'] = '%';
            FaToEn['×'] = '^';

            // Ensure standard unshifted characters retain their lowercase mappings
            FaToEn['ش'] = 'a';
            FaToEn['س'] = 's';
            FaToEn['ی'] = 'd';
            FaToEn['ب'] = 'f';
            FaToEn['ل'] = 'g';
            FaToEn['ا'] = 'h';
            FaToEn['ت'] = 'j';
            FaToEn['ن'] = 'k';
            FaToEn['م'] = 'l';
            FaToEn['ک'] = ';';
            FaToEn['گ'] = '\'';
            FaToEn['ظ'] = 'z';
            FaToEn['ط'] = 'x';
            FaToEn['ز'] = 'c';
            FaToEn['ر'] = 'v';
            FaToEn['ذ'] = 'b';
            FaToEn['د'] = 'n';
            FaToEn['ئ'] = 'm'; // Microsoft layout: m is ئ
            FaToEn['و'] = ',';
            FaToEn['پ'] = '\\';
            FaToEn['ج'] = '[';
            FaToEn['چ'] = ']';
            FaToEn['ح'] = 'p';
            FaToEn['خ'] = 'o';
            FaToEn['ه'] = 'i';
            FaToEn['ع'] = 'u';
            FaToEn['غ'] = 'y';
            FaToEn['ف'] = 't';
            FaToEn['ق'] = 'r';
            FaToEn['ث'] = 'e';
            FaToEn['ص'] = 'w';
            FaToEn['ض'] = 'q';

            // ISIRI layout reverse mappings
            foreach (var kvp in FaToEn)
            {
                FaToEnIsiri[kvp.Key] = kvp.Value;
            }
            FaToEnIsiri['پ'] = 'm';
            FaToEnIsiri['ئ'] = 'M';
            FaToEnIsiri['ژ'] = '\\';
        }

        /// <summary>
        /// Converts the given text automatically detecting whether it's predominantly Persian or English.
        /// </summary>
        public static string Convert(string text, KeyboardLayoutType layout = KeyboardLayoutType.MicrosoftPersian)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            bool isPersianDominant = IsPersianDominant(text);
            return isPersianDominant ? PersianToEnglish(text, layout) : EnglishToPersian(text, layout);
        }

        public static bool IsPersianDominant(string text)
        {
            int persianCount = 0;
            int englishCount = 0;

            foreach (char c in text)
            {
                if (IsPersianChar(c))
                    persianCount++;
                else if (IsEnglishChar(c))
                    englishCount++;
            }

            return persianCount > englishCount;
        }

        private static bool IsPersianChar(char c)
        {
            return (c >= '\u0600' && c <= '\u06FF') || // Arabic/Persian range
                   (c >= '\uFB50' && c <= '\uFDFF') || // Arabic Presentation Forms-A
                   (c >= '\uFE70' && c <= '\uFEFF') || // Arabic Presentation Forms-B
                   (c >= '۰' && c <= '۹') ||
                   c == '؟' || c == '،' || c == '‌';
        }

        private static bool IsEnglishChar(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public static string EnglishToPersian(string text, KeyboardLayoutType layout = KeyboardLayoutType.MicrosoftPersian)
        {
            var sb = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                if (layout == KeyboardLayoutType.ISIRI9147 && IsiriOverrides.TryGetValue(c, out char isiriChar))
                {
                    sb.Append(isiriChar);
                }
                else if (EnToFaNormal.TryGetValue(c, out char faNormal))
                {
                    sb.Append(faNormal);
                }
                else if (EnToFaShifted.TryGetValue(c, out char faShifted))
                {
                    sb.Append(faShifted);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string PersianToEnglish(string text, KeyboardLayoutType layout = KeyboardLayoutType.MicrosoftPersian)
        {
            var sb = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                if (layout == KeyboardLayoutType.ISIRI9147 && FaToEnIsiri.TryGetValue(c, out char isiriEn))
                {
                    sb.Append(isiriEn);
                }
                else if (FaToEn.TryGetValue(c, out char enChar))
                {
                    sb.Append(enChar);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
