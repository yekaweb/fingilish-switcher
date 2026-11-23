using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    public class AppConfig
    {
        public bool Control { get; set; } = true;
        public bool Shift { get; set; } = true;
        public bool Alt { get; set; } = false;
        public bool Win { get; set; } = false;
        public Keys Hotkey { get; set; } = Keys.F; // Default: Ctrl + Shift + F

        public bool EnableFloatingButton { get; set; } = true; // Floating mouse bubble
        public bool StartWithWindows { get; set; } = true;     // Auto-start on boot
        public KeyboardLayoutType LayoutType { get; set; } = KeyboardLayoutType.MicrosoftPersian;
        public bool EnableUndo { get; set; } = true;
        public bool PlayNotificationSound { get; set; } = false;

        public string GetHotkeyDisplayString()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (Control) parts.Add("Ctrl");
            if (Shift) parts.Add("Shift");
            if (Alt) parts.Add("Alt");
            if (Win) parts.Add("Win");
            parts.Add(Hotkey.ToString());
            return string.Join(" + ", parts);
        }
    }

    public static class ConfigManager
    {
        private static readonly string AppDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FingilishSwitcher"
        );
        private static readonly string ConfigPath = Path.Combine(AppDir, "config.json");

        public static AppConfig Current { get; private set; } = new();

        public static void Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    if (config != null)
                    {
                        Current = config;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load config: {ex.Message}");
            }

            Current = new AppConfig();
            Save();
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(AppDir);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Current, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save config: {ex.Message}");
            }
        }
    }
}
