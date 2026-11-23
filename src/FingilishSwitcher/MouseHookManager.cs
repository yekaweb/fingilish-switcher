using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FingilishSwitcher
{
    public class MouseHookManager : IDisposable
    {
        private IntPtr _hookId = IntPtr.Zero;
        private static NativeMethods.LowLevelMouseProc? _proc;
        private readonly Action<int, int, IntPtr> _onSelectionDetected;
        private readonly Action _onDismiss;

        private NativeMethods.POINT _mouseDownPoint;
        private DateTime _lastMouseDownTime = DateTime.MinValue;

        public MouseHookManager(Action<int, int, IntPtr> onSelectionDetected, Action onDismiss)
        {
            _onSelectionDetected = onSelectionDetected;
            _onDismiss = onDismiss;
            // Root the delegate statically to prevent GC collection
            _proc = HookCallback;
        }

        public void Start()
        {
            if (_hookId == IntPtr.Zero)
            {
                try
                {
                    using var curProcess = Process.GetCurrentProcess();
                    using var curModule = curProcess.MainModule;
                    IntPtr hMod = NativeMethods.GetModuleHandle(curModule?.ModuleName);
                    if (hMod == IntPtr.Zero)
                    {
                        hMod = NativeMethods.GetModuleHandle(null);
                    }

                    _hookId = NativeMethods.SetWindowsHookEx(
                        NativeMethods.WH_MOUSE_LL,
                        _proc!,
                        hMod,
                        0
                    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to install mouse hook: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                try
                {
                    int msg = wParam.ToInt32();
                    var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);

                    if (msg == NativeMethods.WM_LBUTTONDOWN)
                    {
                        _lastMouseDownTime = DateTime.Now;
                        _mouseDownPoint = hookStruct.pt;
                    }
                    else if (msg == NativeMethods.WM_LBUTTONUP)
                    {
                        int dx = hookStruct.pt.x - _mouseDownPoint.x;
                        int dy = hookStruct.pt.y - _mouseDownPoint.y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        // Deliberate text selection drag: distance >= 16 pixels.
                        // Ordinary clicks or double-clicks on icons/folders have distance < 4 pixels.
                        if (distance >= 16)
                        {
                            IntPtr hwndUnderMouse = NativeMethods.WindowFromPoint(hookStruct.pt);
                            if (!IsDesktop(hwndUnderMouse))
                            {
                                IntPtr activeFgWindow = NativeMethods.GetForegroundWindow();
                                int targetX = hookStruct.pt.x + 12;
                                int targetY = hookStruct.pt.y - 34;

                                _onSelectionDetected?.Invoke(targetX, targetY, activeFgWindow);
                            }
                        }
                    }
                    else if (msg == NativeMethods.WM_RBUTTONUP || msg == NativeMethods.WM_MBUTTONUP)
                    {
                        _onDismiss?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"MouseHook exception swallowed: {ex.Message}");
                }
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static bool IsDesktop(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return false;

            try
            {
                var sb = new StringBuilder(256);
                NativeMethods.GetClassName(hwnd, sb, 256);
                string className = sb.ToString();

                return className.Equals("Progman", StringComparison.OrdinalIgnoreCase) ||
                       className.Equals("WorkerW", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
