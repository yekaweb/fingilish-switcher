using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace FingilishSwitcher
{
    public class MouseHookManager : IDisposable
    {
        private IntPtr _hookId = IntPtr.Zero;
        private readonly NativeMethods.LowLevelMouseProc _proc;
        private readonly Action<int, int, IntPtr> _onSelectionDetected;
        private readonly Action _onDismiss;
        private readonly SynchronizationContext? _syncContext;

        private NativeMethods.POINT _mouseDownPoint;
        private DateTime _lastMouseDownTime = DateTime.MinValue;

        public MouseHookManager(Action<int, int, IntPtr> onSelectionDetected, Action onDismiss)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _onSelectionDetected = onSelectionDetected;
            _onDismiss = onDismiss;
            _proc = HookCallback;
        }

        public void Start()
        {
            if (_hookId == IntPtr.Zero)
            {
                using var curProcess = Process.GetCurrentProcess();
                using var curModule = curProcess.MainModule;
                _hookId = NativeMethods.SetWindowsHookEx(
                    NativeMethods.WH_MOUSE_LL,
                    _proc,
                    NativeMethods.GetModuleHandle(curModule?.ModuleName),
                    0
                );
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

                    // ONLY trigger on deliberate text drag (horizontal or substantial movement >= 22px).
                    // NEVER trigger on simple double clicks or clicks!
                    if (distance >= 22 && Math.Abs(dy) < 60)
                    {
                        // Check window under point to exclude Desktop icon drags & folder icon clicks
                        IntPtr hwndUnderMouse = NativeMethods.WindowFromPoint(hookStruct.pt);
                        if (!IsDesktopOrExplorerBackground(hwndUnderMouse))
                        {
                            IntPtr activeFgWindow = NativeMethods.GetForegroundWindow();
                            int targetX = hookStruct.pt.x + 10;
                            int targetY = hookStruct.pt.y - 28;

                            _syncContext?.Post(_ =>
                            {
                                _onSelectionDetected?.Invoke(targetX, targetY, activeFgWindow);
                            }, null);
                        }
                    }
                }
                else if (msg == NativeMethods.WM_RBUTTONUP || msg == NativeMethods.WM_MBUTTONUP)
                {
                    _syncContext?.Post(_ =>
                    {
                        _onDismiss?.Invoke();
                    }, null);
                }
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static bool IsDesktopOrExplorerBackground(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return false;

            var sb = new StringBuilder(256);
            NativeMethods.GetClassName(hwnd, sb, 256);
            string className = sb.ToString();

            // Exclude desktop and explorer folder background selection rectangles
            if (className.Equals("Progman", StringComparison.OrdinalIgnoreCase) ||
                className.Equals("WorkerW", StringComparison.OrdinalIgnoreCase) ||
                className.Equals("SHELLDLL_DefView", StringComparison.OrdinalIgnoreCase) ||
                className.Equals("SysListView32", StringComparison.OrdinalIgnoreCase) && IsDesktopHwnd(hwnd))
            {
                return true;
            }

            return false;
        }

        private static bool IsDesktopHwnd(IntPtr hwnd)
        {
            var sb = new StringBuilder(256);
            NativeMethods.GetClassName(hwnd, sb, 256);
            return sb.ToString().Contains("Shell", StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
