using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenuStrip _contextMenu;
        private readonly HotkeyManager _hotkeyManager;
        private readonly FloatingBubbleForm _bubbleForm;
        private readonly MouseHookManager _mouseHookManager;
        private readonly ActivationMessageFilter _messageFilter;

        private ToolStripMenuItem _menuItemToggle = null!;
        private ToolStripMenuItem _menuItemFloating = null!;
        private ToolStripMenuItem _menuItemStartup = null!;
        private ToolStripMenuItem _menuItemShortcutInfo = null!;

        private bool _isEnabled = true;
        private MainDashboardForm? _dashboardForm;

        public static uint WM_SHOW_MAIN_WINDOW { get; private set; }

        public TrayApplicationContext()
        {
            // Register inter-process message for single instance activation
            WM_SHOW_MAIN_WINDOW = NativeMethods.RegisterWindowMessage("FINGILISH_SWITCHER_SHOW_WINDOW_MSG");

            // Load configuration
            ConfigManager.Load();

            // Setup Startup if configured
            if (ConfigManager.Current.StartWithWindows && !StartupManager.IsSetToRunWithWindows())
            {
                StartupManager.SetRunWithWindows(true);
            }

            // Create Context Menu for Tray Icon
            _contextMenu = new ContextMenuStrip
            {
                RightToLeft = RightToLeft.Yes,
                Font = FontManager.GetVazir(9f, FontStyle.Regular)
            };
            BuildContextMenu();

            // Create Tray Icon with custom generated GDI+ Icon
            _trayIcon = new NotifyIcon
            {
                Icon = GenerateTrayIcon(true),
                ContextMenuStrip = _contextMenu,
                Text = "Fingilish Switcher — سامانه تبدیل کیبورد",
                Visible = true
            };
            _trayIcon.DoubleClick += (s, e) => ShowDashboard();

            // Create Floating Bubble with target window forwarding
            _bubbleForm = new FloatingBubbleForm((targetWindow) =>
            {
                if (_isEnabled)
                {
                    _ = TextReplacer.ConvertSelectedTextAsync(targetWindow);
                }
            });

            // Create Hotkey Manager
            _hotkeyManager = new HotkeyManager(() =>
            {
                if (_isEnabled)
                {
                    // Target active foreground window directly
                    IntPtr fg = NativeMethods.GetForegroundWindow();
                    _ = TextReplacer.ConvertSelectedTextAsync(fg);
                }
            });
            RegisterHotkey();

            // Create Mouse Hook Manager
            _mouseHookManager = new MouseHookManager(
                (x, y, targetWindow) =>
                {
                    if (_isEnabled && ConfigManager.Current.EnableFloatingButton)
                    {
                        _bubbleForm.ShowAt(x, y, targetWindow);
                    }
                },
                () =>
                {
                    _bubbleForm.Hide();
                }
            );

            if (ConfigManager.Current.EnableFloatingButton)
            {
                _mouseHookManager.Start();
            }

            // Install message filter to listen for cross-process activation messages
            _messageFilter = new ActivationMessageFilter(WM_SHOW_MAIN_WINDOW, ShowDashboard);
            Application.AddMessageFilter(_messageFilter);

            // Show dashboard on first direct interactive start
            ShowDashboard();
        }

        private void BuildContextMenu()
        {
            _contextMenu.Items.Clear();

            var miOpenDashboard = new ToolStripMenuItem(
                "⚡ باز کردن پیشخوان برنامه (Dashboard)",
                null,
                (s, e) => ShowDashboard()
            )
            {
                Font = FontManager.GetVazir(9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 132, 199)
            };
            _contextMenu.Items.Add(miOpenDashboard);

            _menuItemShortcutInfo = new ToolStripMenuItem(
                $"🔄 تبدیل متن انتخابی ({ConfigManager.Current.GetHotkeyDisplayString()})",
                null,
                async (s, e) =>
                {
                    if (_isEnabled)
                        await TextReplacer.ConvertSelectedTextAsync(NativeMethods.GetForegroundWindow());
                }
            );
            _contextMenu.Items.Add(_menuItemShortcutInfo);

            _contextMenu.Items.Add(new ToolStripSeparator());

            _menuItemToggle = new ToolStripMenuItem("فعال بودن سامانه تبدیل", null, (s, e) => ToggleEnabled())
            {
                Checked = _isEnabled,
                CheckOnClick = true
            };
            _contextMenu.Items.Add(_menuItemToggle);

            _menuItemFloating = new ToolStripMenuItem("دکمه شناور موس هنگام انتخاب متن", null, (s, e) => ToggleFloatingButton())
            {
                Checked = ConfigManager.Current.EnableFloatingButton,
                CheckOnClick = true
            };
            _contextMenu.Items.Add(_menuItemFloating);

            _menuItemStartup = new ToolStripMenuItem("اجرای خودکار با ویندوز (Startup)", null, (s, e) => ToggleStartup())
            {
                Checked = StartupManager.IsSetToRunWithWindows(),
                CheckOnClick = true
            };
            _contextMenu.Items.Add(_menuItemStartup);

            _contextMenu.Items.Add(new ToolStripSeparator());

            _contextMenu.Items.Add(new ToolStripMenuItem("ℹ️ راهنما و درباره", null, (s, e) => ShowAbout()));

            _contextMenu.Items.Add(new ToolStripSeparator());

            _contextMenu.Items.Add(new ToolStripMenuItem("❌ خروج کامل", null, (s, e) => ExitApplication()));
        }

        public void ShowDashboard()
        {
            if (_dashboardForm == null || _dashboardForm.IsDisposed)
            {
                _dashboardForm = new MainDashboardForm();
                _dashboardForm.SettingsUpdated += () =>
                {
                    _menuItemShortcutInfo.Text = $"🔄 تبدیل متن انتخابی ({ConfigManager.Current.GetHotkeyDisplayString()})";
                    _menuItemFloating.Checked = ConfigManager.Current.EnableFloatingButton;
                    _menuItemStartup.Checked = StartupManager.IsSetToRunWithWindows();

                    if (ConfigManager.Current.EnableFloatingButton && _isEnabled)
                        _mouseHookManager.Start();
                    else
                    {
                        _mouseHookManager.Stop();
                        _bubbleForm.Hide();
                    }

                    RegisterHotkey();
                };
            }

            _dashboardForm.Show();
            _dashboardForm.WindowState = FormWindowState.Normal;
            _dashboardForm.BringToFront();
            _dashboardForm.Activate();
            NativeMethods.SetForegroundWindow(_dashboardForm.Handle);
        }

        private void RegisterHotkey()
        {
            bool success = _hotkeyManager.Register(ConfigManager.Current);
            if (!success)
            {
                _trayIcon.ShowBalloonTip(
                    3000,
                    "خطا در ثبت کلید میانبر",
                    $"کلید میانبر {ConfigManager.Current.GetHotkeyDisplayString()} احتمالاً توسط برنامه دیگری در حال استفاده است. لطفاً از طریق پیشخوان کلید دیگری را انتخاب کنید.",
                    ToolTipIcon.Warning
                );
            }
        }

        private void ToggleEnabled()
        {
            _isEnabled = !_isEnabled;
            _menuItemToggle.Checked = _isEnabled;
            _trayIcon.Icon = GenerateTrayIcon(_isEnabled);

            if (_isEnabled)
            {
                if (ConfigManager.Current.EnableFloatingButton)
                    _mouseHookManager.Start();
                RegisterHotkey();
                _trayIcon.Text = "Fingilish Switcher (فعال)";
            }
            else
            {
                _mouseHookManager.Stop();
                _hotkeyManager.Unregister();
                _bubbleForm.Hide();
                _trayIcon.Text = "Fingilish Switcher (متوقف‌شده)";
            }
        }

        private void ToggleFloatingButton()
        {
            ConfigManager.Current.EnableFloatingButton = _menuItemFloating.Checked;
            ConfigManager.Save();

            if (ConfigManager.Current.EnableFloatingButton && _isEnabled)
                _mouseHookManager.Start();
            else
            {
                _mouseHookManager.Stop();
                _bubbleForm.Hide();
            }
        }

        private void ToggleStartup()
        {
            bool newState = _menuItemStartup.Checked;
            StartupManager.SetRunWithWindows(newState);
            ConfigManager.Current.StartWithWindows = newState;
            ConfigManager.Save();
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "⚡ نرم‌افزار Fingilish Switcher Industrial Edition\nنسخه ۱.۲.۰\n\n" +
                "تبدیل هوشمند دوطرفه متون اشتباه تایپ‌شده بین کیبورد فارسی و انگلیسی.\n\n" +
                "• آیکون بازطراحی‌شده صنعتی و مدرن\n" +
                "• تایپوگرافی اصیل وزیر متن (Vazirmatn)\n" +
                "• کلید میانبر پیش‌فرض: " + ConfigManager.Current.GetHotkeyDisplayString() + "\n" +
                "• دکمه شناور موس: " + (ConfigManager.Current.EnableFloatingButton ? "فعال (فقط هنگام درگ متن)" : "غیرفعال") + "\n" +
                "• مصرف بهینه‌سازی‌شده: کمتر از ۱۵ مگابایت حافظه و ۰.۰٪ پردازنده در حالت آماده‌باش.",
                "درباره Fingilish Switcher",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ExitApplication()
        {
            Application.RemoveMessageFilter(_messageFilter);
            _mouseHookManager.Dispose();
            _hotkeyManager.Dispose();
            _bubbleForm.Dispose();
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            _dashboardForm?.Dispose();
            ExitThread();
        }

        private static Icon GenerateTrayIcon(bool active)
        {
            try
            {
                string icoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (System.IO.File.Exists(icoPath))
                {
                    return new Icon(icoPath, 32, 32);
                }
            }
            catch { }

            using var bitmap = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Color bg = active ? Color.FromArgb(37, 99, 235) : Color.FromArgb(100, 116, 139);
                using (var brush = new SolidBrush(bg))
                {
                    g.FillEllipse(brush, 1, 1, 30, 30);
                }

                using (var pen = new Pen(Color.White, 2f))
                {
                    g.DrawEllipse(pen, 2, 2, 28, 28);
                }

                using (var font = new Font("Segoe UI", 14f, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString("F", font, brush, new RectangleF(0, 0, 32, 32), sf);
                }
            }

            return Icon.FromHandle(bitmap.GetHicon());
        }

        private class ActivationMessageFilter : IMessageFilter
        {
            private readonly uint _targetMsg;
            private readonly Action _onActivate;

            public ActivationMessageFilter(uint targetMsg, Action onActivate)
            {
                _targetMsg = targetMsg;
                _onActivate = onActivate;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == _targetMsg)
                {
                    _onActivate?.Invoke();
                    return true;
                }
                return false;
            }
        }
    }
}
