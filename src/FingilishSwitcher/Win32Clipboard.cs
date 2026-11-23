using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace FingilishSwitcher
{
    public static class Win32Clipboard
    {
        public static string? GetText()
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                if (NativeMethods.OpenClipboard(IntPtr.Zero))
                {
                    try
                    {
                        if (NativeMethods.IsClipboardFormatAvailable(NativeMethods.CF_UNICODETEXT))
                        {
                            IntPtr hData = NativeMethods.GetClipboardData(NativeMethods.CF_UNICODETEXT);
                            if (hData != IntPtr.Zero)
                            {
                                IntPtr pText = NativeMethods.GlobalLock(hData);
                                if (pText != IntPtr.Zero)
                                {
                                    try
                                    {
                                        return Marshal.PtrToStringUni(pText);
                                    }
                                    finally
                                    {
                                        NativeMethods.GlobalUnlock(hData);
                                    }
                                }
                            }
                        }
                        return null;
                    }
                    finally
                    {
                        NativeMethods.CloseClipboard();
                    }
                }
                Thread.Sleep(20);
            }
            return null;
        }

        public static bool SetText(string text)
        {
            if (text == null) return false;

            for (int attempt = 0; attempt < 10; attempt++)
            {
                if (NativeMethods.OpenClipboard(IntPtr.Zero))
                {
                    try
                    {
                        NativeMethods.EmptyClipboard();

                        // Unicode text requires UTF-16 characters + null terminator
                        int byteCount = (text.Length + 1) * 2;
                        IntPtr hGlobal = NativeMethods.GlobalAlloc(NativeMethods.GMEM_MOVEABLE, (UIntPtr)byteCount);
                        if (hGlobal == IntPtr.Zero)
                        {
                            return false;
                        }

                        IntPtr pTarget = NativeMethods.GlobalLock(hGlobal);
                        if (pTarget != IntPtr.Zero)
                        {
                            try
                            {
                                byte[] bytes = Encoding.Unicode.GetBytes(text + "\0");
                                Marshal.Copy(bytes, 0, pTarget, bytes.Length);
                            }
                            finally
                            {
                                NativeMethods.GlobalUnlock(hGlobal);
                            }
                        }

                        NativeMethods.SetClipboardData(NativeMethods.CF_UNICODETEXT, hGlobal);
                        return true;
                    }
                    finally
                    {
                        NativeMethods.CloseClipboard();
                    }
                }
                Thread.Sleep(20);
            }
            return false;
        }
    }
}
