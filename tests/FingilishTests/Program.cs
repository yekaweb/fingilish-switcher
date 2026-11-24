using System;
using System.Collections.Generic;
using FingilishSwitcher;

namespace FingilishTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Fingilish Switcher Industrial & Polyglot Test Suite ===");
            int passed = 0;
            int total = 0;

            void AssertEqual(string name, string actual, string expected)
            {
                total++;
                if (actual == expected)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[PASS] {name}");
                    Console.ResetColor();
                    passed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[FAIL] {name}");
                    Console.WriteLine($"   Expected: \"{expected}\"");
                    Console.WriteLine($"   Actual:   \"{actual}\"");
                    Console.ResetColor();
                }
            }

            void AssertTrue(string name, bool condition)
            {
                total++;
                if (condition)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[PASS] {name}");
                    Console.ResetColor();
                    passed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[FAIL] {name} (expected true)");
                    Console.ResetColor();
                }
            }

            // -------------------------------------------------------------
            // SECTION 1: Base Persian / English Tests
            // -------------------------------------------------------------
            // Test 1: User's Example 1 (English typed inadvertently, should convert to Persian)
            string userExample1 = "sghl phgj o,fi? lkl o,fl";
            string expectedFa1 = "سلام حالت خوبه؟ منم خوبم";
            string result1 = KeyboardMapper.Convert(userExample1);
            AssertEqual("User Example 1: En -> Fa", result1, expectedFa1);

            // Test 2: User's Example 2 (Persian typed inadvertently, should convert to English)
            string userExample2 = "اخص شقث غخع؟هئ لخخی";
            string expectedEn2 = "how are you?im good";
            string result2 = KeyboardMapper.Convert(userExample2);
            AssertEqual("User Example 2: Fa -> En", result2, expectedEn2);

            // Test 3: Auto-detection direction verification
            AssertEqual("Detect En Dominance", KeyboardMapper.IsPersianDominant(userExample1).ToString(), "False");
            AssertEqual("Detect Fa Dominance", KeyboardMapper.IsPersianDominant(userExample2).ToString(), "True");

            // Test 4: Shifted characters and Persian special letters (H -> آ, C -> ژ, B -> نیم‌فاصله, ? -> ؟)
            string shiftedEn = "Hgh Ckn B?";
            string expectedFaShifted = "آلا ژند ‌؟";
            string resultShifted = KeyboardMapper.EnglishToPersian(shiftedEn);
            AssertEqual("Shifted characters (آ, ژ, نیم‌فاصله, ؟)", resultShifted, expectedFaShifted);

            // Test 5: Reverse of shifted characters
            string reverseShifted = KeyboardMapper.PersianToEnglish(expectedFaShifted);
            AssertEqual("Reverse shifted characters", reverseShifted, shiftedEn);

            // Test 6: Numbers En -> Fa & Fa -> En
            string numbers = "1234567890";
            string expectedFaNumbers = "۱۲۳۴۵۶۷۸۹۰";
            string resultNumbers = KeyboardMapper.EnglishToPersian(numbers);
            AssertEqual("Numbers En -> Fa", resultNumbers, expectedFaNumbers);

            string reverseNumbers = KeyboardMapper.PersianToEnglish(expectedFaNumbers);
            AssertEqual("Numbers Fa -> En", reverseNumbers, numbers);

            // Test 7: ISIRI 9147 Layout mode check
            string isiriEn = "m";
            string expectedIsiriFa = "پ";
            string isiriResult = KeyboardMapper.EnglishToPersian(isiriEn, KeyboardLayoutType.ISIRI9147);
            AssertEqual("ISIRI 9147 Layout Override (m -> پ)", isiriResult, expectedIsiriFa);

            // Test 8: Empty / Whitespace preservation
            AssertEqual("Empty string", KeyboardMapper.Convert(""), "");
            AssertEqual("Spaces and newlines", KeyboardMapper.Convert("   \r\n   "), "   \r\n   ");

            // -------------------------------------------------------------
            // SECTION 2: Dynamic Locale Discovery (Win32)
            // -------------------------------------------------------------
            var installedLocales = LocaleManager.GetInstalledLocales();
            AssertTrue("LocaleManager detects at least 1 OS layout", installedLocales.Count > 0);
            foreach (var loc in installedLocales)
            {
                Console.WriteLine($"   * Detected OS Layout: {loc.Badge} [{loc.Code}] - {loc.EnglishName} ({loc.NativeName}) {loc.Flag}");
            }

            // -------------------------------------------------------------
            // SECTION 3: Russian JCUKEN Layout Tests
            // -------------------------------------------------------------
            // Test 9: Russian basic greeting "ghbdtn" -> "привет"
            string enRu = "ghbdtn";
            string expectedRu = "привет";
            AssertEqual("Russian conversion: ghbdtn -> привет", MultiKeyboardMapper.EnglishToRussian(enRu), expectedRu);
            AssertEqual("Russian reverse: привет -> ghbdtn", MultiKeyboardMapper.RussianToEnglish(expectedRu), enRu);

            // Test 10: Russian phrase "rfr ltkf" -> "как дела"
            string enRuPhrase = "rfr ltkf";
            string expectedRuPhrase = "как дела";
            AssertEqual("Russian phrase: rfr ltkf -> как дела", MultiKeyboardMapper.EnglishToRussian(enRuPhrase), expectedRuPhrase);

            // Test 11: Russian Uppercase/Shifted
            string enRuUpper = "QWERTY";
            string expectedRuUpper = "ЙЦУКЕН";
            AssertEqual("Russian uppercase: QWERTY -> ЙЦУКЕН", MultiKeyboardMapper.EnglishToRussian(enRuUpper), expectedRuUpper);

            // -------------------------------------------------------------
            // SECTION 4: Spanish Layout Tests
            // -------------------------------------------------------------
            // Test 12: Spanish ñ: "espa;ol" -> "español"
            string enEs = "espa;ol";
            string expectedEs = "español";
            AssertEqual("Spanish conversion: espa;ol -> español", MultiKeyboardMapper.EnglishToSpanish(enEs), expectedEs);
            AssertEqual("Spanish reverse: español -> espa;ol", MultiKeyboardMapper.SpanishToEnglish(expectedEs), enEs);

            // Test 13: Spanish inverted marks
            string enEsPunct = "=Como estas+";
            string expectedEsPunct = "¿Como estas¡";
            AssertEqual("Spanish punctuation: =Como estas+ -> ¿Como estas¡", MultiKeyboardMapper.EnglishToSpanish(enEsPunct), expectedEsPunct);

            // -------------------------------------------------------------
            // SECTION 5: Script Detection Tests
            // -------------------------------------------------------------
            AssertEqual("Detect Cyrillic script", MultiKeyboardMapper.DetectScript("привет").ToString(), "Cyrillic");
            AssertEqual("Detect PersianArabic script", MultiKeyboardMapper.DetectScript("سلام دنیا").ToString(), "PersianArabic");
            AssertEqual("Detect Latin script", MultiKeyboardMapper.DetectScript("Hello world").ToString(), "Latin");
            AssertEqual("Detect Korean script", MultiKeyboardMapper.DetectScript("한국어 사랑").ToString(), "Korean");

            // -------------------------------------------------------------
            // SECTION 6: Korean 2-Set Hangul Automaton Tests
            // -------------------------------------------------------------
            // Test 14: Basic syllable "rk" -> "가"
            AssertEqual("Korean basic: rk -> 가", HangulAutomaton.Compose("rk"), "가");
            AssertEqual("Korean reverse: 가 -> rk", HangulAutomaton.Decompose("가"), "rk");

            // Test 15: Syllable with batchim "gks" -> "한"
            AssertEqual("Korean batchim: gks -> 한", HangulAutomaton.Compose("gks"), "한");
            AssertEqual("Korean reverse: 한 -> gks", HangulAutomaton.Decompose("한"), "gks");

            // Test 16: Multi-syllable word "gksrnr" -> "한국"
            AssertEqual("Korean word: gksrnr -> 한국", HangulAutomaton.Compose("gksrnr"), "한국");
            AssertEqual("Korean reverse: 한국 -> gksrnr", HangulAutomaton.Decompose("한국"), "gksrnr");

            // Test 17: Korean "한글" (gksrmf)
            AssertEqual("Korean word: gksrmf -> 한글", HangulAutomaton.Compose("gksrmf"), "한글");
            AssertEqual("Korean reverse: 한글 -> gksrmf", HangulAutomaton.Decompose("한글"), "gksrmf");

            // Test 18: Korean "사랑" (tkfkd)
            AssertEqual("Korean word: tkfkd -> 사랑", HangulAutomaton.Compose("tkfkd"), "사랑");
            AssertEqual("Korean reverse: 사랑 -> tkfkd", HangulAutomaton.Decompose("사랑"), "tkfkd");

            // Test 19: Batchim transfer to next vowel "gksl" (하 + 니 -> 하니)
            AssertEqual("Korean batchim transfer: gksl -> 하니", HangulAutomaton.Compose("gksl"), "하니");
            AssertEqual("Korean word: gksdl -> 한이", HangulAutomaton.Compose("gksdl"), "한이");

            // Test 20: Universal MultiKeyboardMapper with Korean
            AssertEqual("MultiKeyboardMapper: gksrnr -> 한국", MultiKeyboardMapper.Convert("gksrnr", "ko"), "한국");
            AssertEqual("MultiKeyboardMapper: 한국 -> gksrnr", MultiKeyboardMapper.Convert("한국", "en"), "gksrnr");

            // -------------------------------------------------------------
            // SECTION 7: Multi-Way Cycle Morphing
            // -------------------------------------------------------------
            var mockLocales = new List<InstalledLocale>
            {
                LocaleManager.CreateLocaleFromLcid(IntPtr.Zero, 0x0409), // EN
                LocaleManager.CreateLocaleFromLcid(IntPtr.Zero, 0x0419), // RU
                LocaleManager.CreateLocaleFromLcid(IntPtr.Zero, 0x0429)  // FA
            };

            string cycleText = "sghl";
            string targetCode = "en";

            // Cycle 1: EN -> RU
            string cycle1 = MultiKeyboardMapper.CycleConvert(cycleText, mockLocales, ref targetCode, out string badge1);
            AssertEqual("Cycle 1 target badge", badge1, "RU");
            AssertEqual("Cycle 1 converted to Russian", cycle1, "ыпрд");

            // Cycle 2: RU -> FA
            string cycle2 = MultiKeyboardMapper.CycleConvert(cycleText, mockLocales, ref targetCode, out string badge2);
            AssertEqual("Cycle 2 target badge", badge2, "FA");
            AssertEqual("Cycle 2 converted to Persian", cycle2, "سلام");

            // Cycle 3: FA -> EN
            string cycle3 = MultiKeyboardMapper.CycleConvert(cycleText, mockLocales, ref targetCode, out string badge3);
            AssertEqual("Cycle 3 target badge", badge3, "EN");
            AssertEqual("Cycle 3 converted back to English", cycle3, "sghl");

            Console.WriteLine();
            Console.WriteLine($"Results: {passed} / {total} tests passed.");

            if (passed == total)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ALL INDUSTRIAL & POLYGLOT TESTS PASSED SUCCESSFULLY! ✔");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("SOME TESTS FAILED! ❌");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }
    }
}
