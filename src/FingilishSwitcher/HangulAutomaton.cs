using System;
using System.Collections.Generic;
using System.Text;

namespace FingilishSwitcher
{
    /// <summary>
    /// Mathematical 2-Set (두벌식) Hangul Syllable Automaton.
    /// Performs in-place syllable composition and decomposition using standard Unicode formulas.
    /// </summary>
    public static class HangulAutomaton
    {
        private const int HANGUL_BASE = 0xAC00;
        private const int HANGUL_END = 0xD7A3;

        // 19 Choseong (Initial Consonants)
        private static readonly string[] Choseong =
        {
            "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ",
            "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
        };

        // 21 Jungseong (Medial Vowels)
        private static readonly string[] Jungseong =
        {
            "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ",
            "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ"
        };

        // 28 Jongseong (Final Consonants / Batchim, 0 is empty)
        private static readonly string[] Jongseong =
        {
            "", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ",
            "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ",
            "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
        };

        // Key to Initial Consonant index
        private static readonly Dictionary<char, int> KeyToCho = new()
        {
            {'r', 0},  {'R', 1},  {'s', 2},  {'e', 3},  {'E', 4},
            {'f', 5},  {'a', 6},  {'q', 7},  {'Q', 8},  {'t', 9},
            {'T', 10}, {'d', 11}, {'w', 12}, {'W', 13}, {'c', 14},
            {'z', 15}, {'x', 16}, {'v', 17}, {'g', 18}
        };

        // Key to Medial Vowel index
        private static readonly Dictionary<char, int> KeyToJung = new()
        {
            {'k', 0},  {'o', 1},  {'i', 2},  {'O', 3},  {'j', 4},
            {'p', 5},  {'u', 6},  {'P', 7},  {'h', 8},  {'y', 12},
            {'n', 13}, {'b', 17}, {'m', 18}, {'l', 20}
        };

        // Compound Vowels lookup (vowel1, vowel2) -> compound vowel index
        private static readonly Dictionary<(int, int), int> CompoundVowels = new()
        {
            {(8, 0), 9},   // ㅗ + ㅏ = ㅘ
            {(8, 1), 10},  // ㅗ + ㅐ = ㅙ
            {(8, 20), 11}, // ㅗ + ㅣ = ㅚ
            {(13, 4), 14}, // ㅜ + ㅓ = ㅝ
            {(13, 5), 15}, // ㅜ + ㅔ = ㅞ
            {(13, 20), 16},// ㅜ + ㅣ = ㅟ
            {(18, 20), 19} // ㅡ + ㅣ = ㅢ
        };

        // Key to Final Consonant index
        private static readonly Dictionary<char, int> KeyToJong = new()
        {
            {'r', 1},  {'R', 2},  {'s', 4},  {'e', 7},  {'f', 8},
            {'a', 16}, {'q', 17}, {'t', 19}, {'T', 20}, {'d', 21},
            {'w', 22}, {'c', 23}, {'z', 24}, {'x', 25}, {'v', 26}, {'g', 27}
        };

        // Compound Batchim lookup (batchim1, batchim2) -> compound batchim index
        private static readonly Dictionary<(int, int), int> CompoundBatchim = new()
        {
            {(1, 19), 3},   // ㄱ + ㅅ = ㄳ
            {(4, 22), 5},   // ㄴ + ㅈ = ㄵ
            {(4, 27), 6},   // ㄴ + ㅎ = ㄶ
            {(8, 1), 9},    // ㄹ + ㄱ = ㄺ
            {(8, 16), 10},  // ㄹ + ㅁ = ㄻ
            {(8, 17), 11},  // ㄹ + ㅂ = ㄼ
            {(8, 19), 12},  // ㄹ + ㅅ = ㄽ
            {(8, 25), 13},  // ㄹ + ㅌ = ㄾ
            {(8, 26), 14},  // ㄹ + ㅍ = ㄿ
            {(8, 27), 15},  // ㄹ + ㅎ = ㅀ
            {(17, 19), 18}  // ㅂ + ㅅ = ㅄ
        };

        // Decompose compound batchim into two basic batchims
        private static readonly Dictionary<int, (int, int)> SplitBatchim = new()
        {
            {3, (1, 19)},
            {5, (4, 22)},
            {6, (4, 27)},
            {9, (8, 1)},
            {10, (8, 16)},
            {11, (8, 17)},
            {12, (8, 19)},
            {13, (8, 25)},
            {14, (8, 26)},
            {15, (8, 27)},
            {18, (17, 19)}
        };

        // Single Jamo to QWERTY key
        private static readonly Dictionary<char, char> JamoToKey = new()
        {
            // Consonants
            {'ㄱ', 'r'}, {'ㄲ', 'R'}, {'ㄴ', 's'}, {'ㄷ', 'e'}, {'ㄸ', 'E'},
            {'ㄹ', 'f'}, {'ㅁ', 'a'}, {'ㅂ', 'q'}, {'ㅃ', 'Q'}, {'ㅅ', 't'},
            {'ㅆ', 'T'}, {'ㅇ', 'd'}, {'ㅈ', 'w'}, {'ㅉ', 'W'}, {'ㅊ', 'c'},
            {'ㅋ', 'z'}, {'ㅌ', 'x'}, {'ㅍ', 'v'}, {'ㅎ', 'g'},
            // Vowels
            {'ㅏ', 'k'}, {'ㅐ', 'o'}, {'ㅑ', 'i'}, {'ㅒ', 'O'}, {'ㅓ', 'j'},
            {'ㅔ', 'p'}, {'ㅕ', 'u'}, {'ㅖ', 'P'}, {'ㅗ', 'h'}, {'ㅛ', 'y'},
            {'ㅜ', 'n'}, {'ㅠ', 'b'}, {'ㅡ', 'm'}, {'ㅣ', 'l'}
        };

        // Batchim index to Cho index
        private static readonly Dictionary<int, int> JongToCho = new()
        {
            {1, 0},   // ㄱ
            {2, 1},   // ㄲ
            {4, 2},   // ㄴ
            {7, 3},   // ㄷ
            {8, 5},   // ㄹ
            {16, 6},  // ㅁ
            {17, 7},  // ㅂ
            {19, 9},  // ㅅ
            {20, 10}, // ㅆ
            {21, 11}, // ㅇ
            {22, 12}, // ㅈ
            {23, 14}, // ㅊ
            {24, 15}, // ㅋ
            {25, 16}, // ㅌ
            {26, 17}, // ㅍ
            {27, 18}  // ㅎ
        };

        private static char BuildSyllable(int cho, int jung, int jong)
        {
            return (char)(HANGUL_BASE + (cho * 21 + jung) * 28 + jong);
        }

        /// <summary>
        /// Composes QWERTY characters into Korean Hangul syllables using the 2-Set Automaton.
        /// e.g. "gksrnr" -> "한국", "tkfkd" -> "사랑"
        /// </summary>
        public static string Compose(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sb = new StringBuilder();
            int cho = -1;
            int jung = -1;
            int jong = 0;
            int jong1 = 0; // First part of compound batchim

            void Flush()
            {
                if (cho != -1 && jung != -1)
                {
                    sb.Append(BuildSyllable(cho, jung, jong));
                }
                else if (cho != -1)
                {
                    sb.Append(Choseong[cho]);
                }
                else if (jung != -1)
                {
                    sb.Append(Jungseong[jung]);
                }
                cho = -1;
                jung = -1;
                jong = 0;
                jong1 = 0;
            }

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                bool isChoKey = KeyToCho.TryGetValue(c, out int choIdx);
                bool isJungKey = KeyToJung.TryGetValue(c, out int jungIdx);

                if (isChoKey && !isJungKey)
                {
                    // Consonant typed
                    if (cho == -1)
                    {
                        // State 0: New syllable starts with initial consonant
                        cho = choIdx;
                    }
                    else if (jung == -1)
                    {
                        // State 1: Already had initial consonant, got another consonant -> flush first
                        Flush();
                        cho = choIdx;
                    }
                    else if (jong == 0)
                    {
                        // State 2: Had Cho + Jung, got first batchim candidate
                        if (KeyToJong.TryGetValue(c, out int singleJong))
                        {
                            jong = singleJong;
                            jong1 = singleJong;
                        }
                        else
                        {
                            Flush();
                            cho = choIdx;
                        }
                    }
                    else
                    {
                        // State 3: Had Cho + Jung + Jong, check for compound batchim
                        if (KeyToJong.TryGetValue(c, out int secondJong) &&
                            CompoundBatchim.TryGetValue((jong1, secondJong), out int compJong))
                        {
                            jong = compJong;
                        }
                        else
                        {
                            Flush();
                            cho = choIdx;
                        }
                    }
                }
                else if (isJungKey && !isChoKey)
                {
                    // Vowel typed
                    if (cho == -1)
                    {
                        // Standalone vowel
                        if (jung == -1)
                        {
                            jung = jungIdx;
                        }
                        else if (CompoundVowels.TryGetValue((jung, jungIdx), out int compJung))
                        {
                            jung = compJung;
                        }
                        else
                        {
                            Flush();
                            jung = jungIdx;
                        }
                    }
                    else if (jung == -1)
                    {
                        // Cho + Jung formed
                        jung = jungIdx;
                    }
                    else if (jong == 0)
                    {
                        // Check for compound vowel
                        if (CompoundVowels.TryGetValue((jung, jungIdx), out int compJung))
                        {
                            jung = compJung;
                        }
                        else
                        {
                            Flush();
                            jung = jungIdx;
                        }
                    }
                    else
                    {
                        // Had final consonant, but next is a vowel!
                        // The final consonant must be transferred to the new syllable!
                        if (SplitBatchim.TryGetValue(jong, out var parts))
                        {
                            // Compound batchim: leave first part, transfer second part
                            jong = parts.Item1;
                            Flush();
                            cho = JongToCho[parts.Item2];
                            jung = jungIdx;
                        }
                        else if (JongToCho.TryGetValue(jong, out int nextCho))
                        {
                            // Single batchim: remove batchim from previous, make it initial of new
                            jong = 0;
                            Flush();
                            cho = nextCho;
                            jung = jungIdx;
                        }
                        else
                        {
                            Flush();
                            jung = jungIdx;
                        }
                    }
                }
                else
                {
                    // Space, number, or punctuation
                    Flush();
                    sb.Append(c);
                }
            }

            Flush();
            return sb.ToString();
        }

        /// <summary>
        /// Decomposes Korean Hangul syllables back into QWERTY keystrokes.
        /// e.g. "한국" -> "gksrnr", "사랑" -> "tkfkd"
        /// </summary>
        public static string Decompose(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sb = new StringBuilder();

            foreach (char c in input)
            {
                if (c >= HANGUL_BASE && c <= HANGUL_END)
                {
                    int offset = c - HANGUL_BASE;
                    int jongIdx = offset % 28;
                    int jungIdx = (offset / 28) % 21;
                    int choIdx = (offset / 28) / 21;

                    // 1. Initial Consonant
                    string choChar = Choseong[choIdx];
                    if (JamoToKey.TryGetValue(choChar[0], out char choKey))
                        sb.Append(choKey);

                    // 2. Medial Vowel
                    DecomposeJung(jungIdx, sb);

                    // 3. Final Consonant (Batchim)
                    if (jongIdx > 0)
                    {
                        DecomposeJong(jongIdx, sb);
                    }
                }
                else if (JamoToKey.TryGetValue(c, out char k))
                {
                    sb.Append(k);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static void DecomposeJung(int jungIdx, StringBuilder sb)
        {
            switch (jungIdx)
            {
                case 9:  sb.Append("hk"); break; // ㅘ = ㅗ + ㅏ
                case 10: sb.Append("ho"); break; // ㅙ = ㅗ + ㅐ
                case 11: sb.Append("hl"); break; // ㅚ = ㅗ + ㅣ
                case 14: sb.Append("nj"); break; // ㅝ = ㅜ + ㅓ
                case 15: sb.Append("np"); break; // ㅞ = ㅜ + ㅔ
                case 16: sb.Append("nl"); break; // ㅟ = ㅜ + ㅣ
                case 19: sb.Append("ml"); break; // ㅢ = ㅡ + ㅣ
                default:
                    string j = Jungseong[jungIdx];
                    if (JamoToKey.TryGetValue(j[0], out char k))
                        sb.Append(k);
                    break;
            }
        }

        private static void DecomposeJong(int jongIdx, StringBuilder sb)
        {
            switch (jongIdx)
            {
                case 3:  sb.Append("rt"); break; // ㄳ = ㄱ + ㅅ
                case 5:  sb.Append("sw"); break; // ㄵ = ㄴ + ㅈ
                case 6:  sb.Append("sg"); break; // ㄶ = ㄴ + ㅎ
                case 9:  sb.Append("fr"); break; // ㄺ = ㄹ + ㄱ
                case 10: sb.Append("fa"); break; // ㄻ = ㄹ + ㅁ
                case 11: sb.Append("fq"); break; // ㄼ = ㄹ + ㅂ
                case 12: sb.Append("ft"); break; // ㄽ = ㄹ + ㅅ
                case 13: sb.Append("fx"); break; // ㄾ = ㄹ + ㅌ
                case 14: sb.Append("fv"); break; // ㄿ = ㄹ + ㅍ
                case 15: sb.Append("fg"); break; // ㅀ = ㄹ + ㅎ
                case 18: sb.Append("qt"); break; // ㅄ = ㅂ + ㅅ
                default:
                    string j = Jongseong[jongIdx];
                    if (j.Length > 0 && JamoToKey.TryGetValue(j[0], out char k))
                        sb.Append(k);
                    break;
            }
        }
    }
}
