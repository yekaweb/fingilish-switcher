using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    internal static class Program
    {
        private const string MutexName = "Global\\FingilishSwitcher_SingleInstance_Mutex_v130";
        private static Mutex? _mutex;

        [STAThread]
        static void Main(string[] args)
        {
            _mutex = new Mutex(true, MutexName, out bool isNewInstance);
            if (!isNewInstance)
            {
                // Another instance is already running!
                // Signal running instance to bring the Main Dashboard window to the foreground
                uint showMsg = NativeMethods.RegisterWindowMessage("FINGILISH_SWITCHER_SHOW_WINDOW_MSG");
                if (showMsg != 0)
                {
                    NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST, showMsg, IntPtr.Zero, IntPtr.Zero);
                }
                return;
            }

            try
            {
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    LogCrash("UnhandledException", e.ExceptionObject?.ToString());
                };

                Application.ThreadException += (s, e) =>
                {
                    LogCrash("ThreadException", e.Exception?.ToString());
                };

                bool isSilentStartup = args.Any(a =>
                    a.Equals("--silent", StringComparison.OrdinalIgnoreCase) ||
                    a.Equals("--startup", StringComparison.OrdinalIgnoreCase) ||
                    a.Equals("-s", StringComparison.OrdinalIgnoreCase)
                );

                ApplicationConfiguration.Initialize();
                Application.Run(new TrayApplicationContext(isSilentStartup));
            }
            catch (Exception ex)
            {
                LogCrash("MainException", ex.ToString());
                MessageBox.Show($"خطا در اجرای برنامه:\n{ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                GC.KeepAlive(_mutex);
            }
        }

        private static void LogCrash(string category, string? details)
        {
            try
            {
                string crashPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
                File.AppendAllText(crashPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{category}]\n{details}\n\n");
            }
            catch { }
        }
    }
}
