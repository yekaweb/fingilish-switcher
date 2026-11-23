using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                // Ensure target window is in foreground if provided
                if (targetWindow.HasValue && targetWindow.Value != IntPtr.Zero)
                {
                    IntPtr currentFg = NativeMethods.GetForegroundWindow();
                    if (currentFg != targetWindow.Value)
                    {
                        NativeMethods.SetForegroundWindow(targetWindow.Value);
                        await Task.Delay(30);
                    }
                }

                // 1. Force release any physical modifiers that user might still be holding (Ctrl, Shift, Alt, Win)
                ReleasePhysicalModifiers();
                await Task.Delay(25);

                // 2. Track clipboard sequence number
                uint seqBefore = NativeMethods.GetClipboardSequenceNumber();

                // 3. Send Ctrl+C
                SendCtrlC();

                // 4. Wait for clipboard sequence to change or text to appear
                for (int i = 0; i < 18; i++)
                {
                    await Task.Delay(15);
                    if (NativeMethods.GetClipboardSequenceNumber() != seqBefore)
                    {
                        break;
                    }
                }

                // Small grace period
                await Task.Delay(15);

                // 5. Read selected text with safe retry
                string? selectedText = TryGetClipboardText();

                if (string.IsNullOrEmpty(selectedText))
                {
                    System.Diagnostics.Debug.WriteLine("No text copied or selection empty.");
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

                // 6. Set replacement text to clipboard with safe retry
                if (!TrySetClipboardText(replacementText))
                {
                    await Task.Delay(25);
                    TrySetClipboardText(replacementText);
                }

                await Task.Delay(25);

                // 7. Ensure modifiers are still released before sending Ctrl+V
                ReleasePhysicalModifiers();
                await Task.Delay(15);

                // 8. Send Ctrl+V to paste over selection
                SendCtrlV();

                // Record in history & increment count
                HistoryManager.IncrementTotal();
                HistoryManager.Add(selectedText, replacementText, isPersianDominant);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Text conversion error: {ex.Message}");
            }
        }

        private static void ReleasePhysicalModifiers()
        {
            // Send KeyUp for all modifier keys to prevent modifier bleeding (e.g. Shift+C instead of Ctrl+C)
            NativeMethods.keybd_event((byte)NativeMethods.VK_SHIFT, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_CONTROL, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_MENU, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_LWIN, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)NativeMethods.VK_RWIN, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private static void SendCtrlC()
        {
            var inputs = new NativeMethods.INPUT[4];

            // Ctrl down
            inputs[0].type = NativeMethods.INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = NativeMethods.VK_CONTROL;

            // C down
            inputs[1].type = NativeMethods.INPUT_KEYBOARD;
            inputs[1].u.ki.wVk = NativeMethods.VK_C;

            // C up
            inputs[2].type = NativeMethods.INPUT_KEYBOARD;
            inputs[2].u.ki.wVk = NativeMethods.VK_C;
            inputs[2].u.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            // Ctrl up
            inputs[3].type = NativeMethods.INPUT_KEYBOARD;
            inputs[3].u.ki.wVk = NativeMethods.VK_CONTROL;
            inputs[3].u.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
        }

        private static void SendCtrlV()
        {
            var inputs = new NativeMethods.INPUT[4];

            // Ctrl down
            inputs[0].type = NativeMethods.INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = NativeMethods.VK_CONTROL;

            // V down
            inputs[1].type = NativeMethods.INPUT_KEYBOARD;
            inputs[1].u.ki.wVk = NativeMethods.VK_V;

            // V up
            inputs[2].type = NativeMethods.INPUT_KEYBOARD;
            inputs[2].u.ki.wVk = NativeMethods.VK_V;
            inputs[2].u.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            // Ctrl up
            inputs[3].type = NativeMethods.INPUT_KEYBOARD;
            inputs[3].u.ki.wVk = NativeMethods.VK_CONTROL;
            inputs[3].u.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));
        }

        private static string? TryGetClipboardText()
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    if (Clipboard.ContainsText())
                        return Clipboard.GetText();
                }
                catch
                {
                    Thread.Sleep(15);
                }
            }
            return null;
        }

        private static bool TrySetClipboardText(string text)
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return true;
                }
                catch
                {
                    Thread.Sleep(15);
                }
            }
            return false;
        }
    }
}
