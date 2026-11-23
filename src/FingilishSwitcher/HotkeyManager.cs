using System;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    public class HotkeyManager : NativeWindow, IDisposable
    {
        private const int HOTKEY_ID = 9001;
        private bool _isRegistered = false;
        private readonly Action _onHotkeyPressed;

        public HotkeyManager(Action onHotkeyPressed)
        {
            _onHotkeyPressed = onHotkeyPressed;
            CreateHandle(new CreateParams());
        }

        public bool Register(AppConfig config)
        {
            Unregister();

            uint modifiers = NativeMethods.MOD_NOREPEAT;
            if (config.Control) modifiers |= NativeMethods.MOD_CONTROL;
            if (config.Shift) modifiers |= NativeMethods.MOD_SHIFT;
            if (config.Alt) modifiers |= NativeMethods.MOD_ALT;
            if (config.Win) modifiers |= NativeMethods.MOD_WIN;

            uint vk = (uint)config.Hotkey;

            _isRegistered = NativeMethods.RegisterHotKey(Handle, HOTKEY_ID, modifiers, vk);
            return _isRegistered;
        }

        public void Unregister()
        {
            if (_isRegistered && Handle != IntPtr.Zero)
            {
                NativeMethods.UnregisterHotKey(Handle, HOTKEY_ID);
                _isRegistered = false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                _onHotkeyPressed?.Invoke();
            }

            base.WndProc(ref m);
        }

        public void Dispose()
        {
            Unregister();
            DestroyHandle();
        }
    }
}
