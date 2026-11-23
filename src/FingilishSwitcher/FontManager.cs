using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    public static class FontManager
    {
        private static readonly PrivateFontCollection FontCollection = new();
        private static FontFamily? _vazirFamily;
        public static bool IsVazirLoaded { get; private set; }

        static FontManager()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string fontsDir = Path.Combine(baseDir, "Resources", "Fonts");

                if (!Directory.Exists(fontsDir))
                {
                    // Fallback to relative path during development
                    fontsDir = Path.Combine(baseDir, "..", "..", "..", "Resources", "Fonts");
                }

                if (Directory.Exists(fontsDir))
                {
                    string regularPath = Path.Combine(fontsDir, "Vazirmatn-Regular.ttf");
                    string boldPath = Path.Combine(fontsDir, "Vazirmatn-Bold.ttf");
                    string mediumPath = Path.Combine(fontsDir, "Vazirmatn-Medium.ttf");
                    string semiBoldPath = Path.Combine(fontsDir, "Vazirmatn-SemiBold.ttf");

                    if (File.Exists(regularPath)) FontCollection.AddFontFile(regularPath);
                    if (File.Exists(boldPath)) FontCollection.AddFontFile(boldPath);
                    if (File.Exists(mediumPath)) FontCollection.AddFontFile(mediumPath);
                    if (File.Exists(semiBoldPath)) FontCollection.AddFontFile(semiBoldPath);

                    if (FontCollection.Families.Length > 0)
                    {
                        _vazirFamily = FontCollection.Families[0];
                        IsVazirLoaded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load Vazirmatn fonts: {ex.Message}");
            }
        }

        public static Font GetVazir(float size, FontStyle style = FontStyle.Regular)
        {
            if (IsVazirLoaded && _vazirFamily != null)
            {
                try
                {
                    return new Font(_vazirFamily, size, style, GraphicsUnit.Point);
                }
                catch { }
            }

            // Fallback font
            return new Font("Segoe UI", size, style, GraphicsUnit.Point);
        }
    }
}
