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

            // If registration failed and it's not already default Ctrl + Shift + F, fall back to Ctrl + Shift + F
            if (!_isRegistered && (config.Hotkey != Keys.F || !config.Control || !config.Shift))
            {
                uint fallbackModifiers = NativeMethods.MOD_NOREPEAT | NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT;
                _isRegistered = NativeMethods.RegisterHotKey(Handle, HOTKEY_ID, fallbackModifiers, (uint)Keys.F);
                if (_isRegistered)
                {
                    config.Control = true;
                    config.Shift = true;
                    config.Alt = false;
                    config.Win = false;
                    config.Hotkey = Keys.F;
                    ConfigManager.Save();
                }
            }

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
