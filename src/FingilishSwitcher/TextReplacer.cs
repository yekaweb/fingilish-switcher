using System;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FingilishSwitcher
{
    public static class TextReplacer
    {
        private static string? _lastOriginalText = null;
        private static string? _lastConvertedText = null;
        private static DateTime _lastConvertedTime = DateTime.MinValue;
        private static readonly object _syncLock = new();

        public static async Task ConvertSelectedTextAsync(IntPtr? targetWindow = null)
        {
            try
            {
                // Ensure target window is foreground
                if (targetWindow.HasValue && targetWindow.Value != IntPtr.Zero)
                {
                    IntPtr currentFg = NativeMethods.GetForegroundWindow();
                    if (currentFg != targetWindow.Value)
                    {
                        NativeMethods.SetForegroundWindow(targetWindow.Value);
                        await Task.Delay(40);
                    }
                }

                // Wait for user to release physical modifiers (Ctrl, Shift, Alt, Win)
                // When user presses Ctrl+Shift+F, their fingers are physically down on the keyboard.
                // If we simulate Ctrl+C while Shift is physically held down, Windows sees Ctrl+Shift+C!
                for (int waitMod = 0; waitMod < 20; waitMod++)
                {
                    bool ctrlDown = (NativeMethods.GetAsyncKeyState(NativeMethods.VK_CONTROL) & 0x8000) != 0;
                    bool shiftDown = (NativeMethods.GetAsyncKeyState(NativeMethods.VK_SHIFT) & 0x8000) != 0;
                    bool altDown = (NativeMethods.GetAsyncKeyState(NativeMethods.VK_MENU) & 0x8000) != 0;
                    if (!ctrlDown && !shiftDown && !altDown)
                        break;
                    await Task.Delay(15);
                }

                ReleasePhysicalModifiers();
                await Task.Delay(20);

                // Track clipboard sequence number
                uint seqBefore = NativeMethods.GetClipboardSequenceNumber();

                // Send Ctrl+C
                SendCtrlC();

                // Wait for clipboard to update or timeout (up to 300ms)
                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(15);
                    if (NativeMethods.GetClipboardSequenceNumber() != seqBefore)
                    {
                        break;
                    }
                }

                await Task.Delay(20);

                // Read selected text using pure Win32 API (STA-independent)
                string? selectedText = Win32Clipboard.GetText();

                if (string.IsNullOrEmpty(selectedText))
                {
                    return;
                }

                string replacementText;
                bool isPersianDominant = KeyboardMapper.IsPersianDominant(selectedText);

                lock (_syncLock)
                {
                    var now = DateTime.Now;
                    if (ConfigManager.Current.EnableUndo &&
                        _lastConvertedText != null &&
                        _lastOriginalText != null &&
                        selectedText == _lastConvertedText &&
                        (now - _lastConvertedTime).TotalSeconds < 25)
                    {
                        // Undo action
                        replacementText = _lastOriginalText;
                        _lastConvertedText = null;
                        _lastOriginalText = null;
                    }
                    else
                    {
                        replacementText = KeyboardMapper.Convert(selectedText, ConfigManager.Current.LayoutType);

                        if (replacementText == selectedText)
                            return;

                        _lastOriginalText = selectedText;
                        _lastConvertedText = replacementText;
                        _lastConvertedTime = now;
                    }
                }

                // Set replacement text onto clipboard
                if (!Win32Clipboard.SetText(replacementText))
                {
                    await Task.Delay(25);
                    Win32Clipboard.SetText(replacementText);
                }

                await Task.Delay(30);

                // Release modifiers once more before Ctrl+V
                ReleasePhysicalModifiers();
                await Task.Delay(20);

                // Send Ctrl+V
                SendCtrlV();

                // Record in history & increment count
                HistoryManager.IncrementTotal();
                HistoryManager.Add(selectedText, replacementText, isPersianDominant);

                if (ConfigManager.Current.PlayNotificationSound)
                {
                    try { SystemSounds.Asterisk.Play(); } catch { }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Text conversion error: {ex.Message}");
            }
        }

        private static void ReleasePhysicalModifiers()
        {
            NativeMethods.keybd_event((byte)NativeMethods.VK_SHIFT, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_CONTROL, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_MENU, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_LWIN, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_RWIN, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private static void SendCtrlC()
        {
            NativeMethods.keybd_event((byte)NativeMethods.VK_SHIFT, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_MENU, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);

            NativeMethods.keybd_event((byte)NativeMethods.VK_CONTROL, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_C, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_C, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_CONTROL, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private static void SendCtrlV()
        {
            NativeMethods.keybd_event((byte)NativeMethods.VK_SHIFT, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_MENU, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);

            NativeMethods.keybd_event((byte)NativeMethods.VK_CONTROL, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_V, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_V, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_CONTROL, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
