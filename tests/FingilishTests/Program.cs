using System;
using FingilishSwitcher;

namespace FingilishTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Fingilish Switcher Automated Test Suite ===");
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
            // C = ژ, k = ن, n = د  => Ckn = ژند
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

            Console.WriteLine();
            Console.WriteLine($"Results: {passed} / {total} tests passed.");

            if (passed == total)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ALL TESTS PASSED SUCCESSFULLY! ✔");
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
