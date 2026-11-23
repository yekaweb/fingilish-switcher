using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    internal static class Program
    {
        private const string MutexName = "Global\\FingilishSwitcher_SingleInstance_Mutex_v110";

        [STAThread]
        static void Main()
        {
            using var mutex = new Mutex(true, MutexName, out bool isNewInstance);
            if (!isNewInstance)
            {
                // Another instance is already running!
                // Signal the running instance to bring the Main Dashboard window to the foreground
                uint showMsg = NativeMethods.RegisterWindowMessage("FINGILISH_SWITCHER_SHOW_WINDOW_MSG");
                if (showMsg != 0)
                {
                    NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST, showMsg, IntPtr.Zero, IntPtr.Zero);
                }
                return;
            }

            try
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new TrayApplicationContext());
            }
            catch (Exception ex)
            {
                string crashPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
                File.WriteAllText(crashPath, ex.ToString());
                MessageBox.Show($"خطا در اجرای برنامه:\n{ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
