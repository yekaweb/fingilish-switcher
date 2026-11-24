using System;
using System.Collections.Generic;

namespace FingilishSwitcher
{
    public class ConversionRecord
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string SourceText { get; set; } = "";
        public string ConvertedText { get; set; } = "";
        public string Direction { get; set; } = "EN ➔ FA"; // or FA ➔ EN
    }

    public static class HistoryManager
    {
        private const int MaxRecords = 30;
        private static readonly List<ConversionRecord> _records = new();
        private static readonly object _lock = new();

        public static event Action? HistoryUpdated;

        public static void Add(string source, string converted, bool wasPersianDominant)
        {
            Add(source, converted, wasPersianDominant ? "FA ➔ EN" : "EN ➔ FA");
        }

        public static void Add(string source, string converted, string direction)
        {
            if (string.IsNullOrWhiteSpace(source) || source == converted)
                return;

            lock (_lock)
            {
                _records.Insert(0, new ConversionRecord
                {
                    Timestamp = DateTime.Now,
                    SourceText = source,
                    ConvertedText = converted,
                    Direction = direction
                });

                if (_records.Count > MaxRecords)
                {
                    _records.RemoveAt(_records.Count - 1);
                }
            }

            HistoryUpdated?.Invoke();
        }

        public static IReadOnlyList<ConversionRecord> GetRecords()
        {
            lock (_lock)
            {
                return _records.ToArray();
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _records.Clear();
            }
            HistoryUpdated?.Invoke();
        }

        public static int TotalConversions { get; private set; } = 0;

        public static void IncrementTotal()
        {
            TotalConversions++;
        }
    }
}
