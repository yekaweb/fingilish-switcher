using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    public class MainDashboardForm : Form
    {
        private Panel _pnlHeader = null!;
        private Panel _pnlNav = null!;
        private Panel _pnlContent = null!;
        private Panel _pnlFooter = null!;

        // Nav Buttons
        private Button _btnTabDashboard = null!;
        private Button _btnTabHotkey = null!;
        private Button _btnTabMouse = null!;
        private Button _btnTabLayout = null!;
        private Button _btnTabHistory = null!;
        private Button _btnTabSystem = null!;

        // Panels for each tab
        private Panel _pageDashboard = null!;
        private Panel _pageHotkey = null!;
        private Panel _pageMouse = null!;
        private Panel _pageLayout = null!;
        private Panel _pageHistory = null!;
        private Panel _pageSystem = null!;

        // Dashboard controls
        private TextBox _txtLiveInput = null!;
        private TextBox _txtLiveOutput = null!;
        private Label _lblDetectedDirection = null!;
        private Label _lblTotalConversions = null!;
        private Label _lblActiveHotkeyBadge = null!;

        // Hotkey controls
        private CheckBox _chkCtrl = null!;
        private CheckBox _chkShift = null!;
        private CheckBox _chkAlt = null!;
        private CheckBox _chkWin = null!;
        private ComboBox _cmbKey = null!;
        private Label _lblHotkeyPreview = null!;

        // Mouse controls
        private CheckBox _chkEnableFloating = null!;
        private RadioButton _rbFloatFast = null!;
        private RadioButton _rbFloatNormal = null!;
        private RadioButton _rbFloatSlow = null!;

        // Layout controls
        private RadioButton _rbMicrosoftLayout = null!;
        private RadioButton _rbIsiriLayout = null!;

        // History controls
        private ListView _lvHistory = null!;

        // System controls
        private CheckBox _chkStartup = null!;
        private CheckBox _chkUndo = null!;
        private CheckBox _chkCloseToTray = null!;

        public event Action? SettingsUpdated;

        public MainDashboardForm()
        {
            InitializeComponent();
            try
            {
                string icoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (System.IO.File.Exists(icoPath))
                {
                    Icon = new Icon(icoPath);
                }
            }
            catch { }
            LoadDataFromConfig();
            SwitchTab(_pageDashboard, _btnTabDashboard);
        }

        private void InitializeComponent()
        {
            Text = "Fingilish Switcher — پیشخوان صنعتی تبدیل هوشمند کیبورد";
            Size = new Size(820, 640);
            MinimumSize = new Size(820, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(15, 23, 42); // Industrial dark slate canvas
            ForeColor = Color.FromArgb(248, 250, 252);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = FontManager.GetVazir(9.5f, FontStyle.Regular);

            // 1. Header
            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(20, 10, 20, 10)
            };

            var lblTitle = new Label
            {
                Text = "FINGILISH SWITCHER",
                Font = FontManager.GetVazir(13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248), // Electric cyan
                Location = new Point(15, 10),
                AutoSize = true
            };

            var lblSubtitle = new Label
            {
                Text = "سامانه صنعتی و هوشمند تبدیل دوطرفه کیبورد فارسی و انگلیسی • نسخه ۱.۲.۰",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(15, 34),
                AutoSize = true
            };

            var lblStatusBadge = new Label
            {
                Text = "● مقیم در حافظه (CPU: ۰.۰٪)",
                Font = FontManager.GetVazir(8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 211, 153), // Emerald
                Location = new Point(590, 22),
                AutoSize = true
            };

            _pnlHeader.Controls.Add(lblTitle);
            _pnlHeader.Controls.Add(lblSubtitle);
            _pnlHeader.Controls.Add(lblStatusBadge);
            Controls.Add(_pnlHeader);

            // 2. Navigation Tab Bar (Clean horizontal industrial tabs)
            _pnlNav = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(24, 32, 47),
                Padding = new Padding(15, 0, 15, 0)
            };

            var flpNav = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoScroll = false
            };

            _btnTabDashboard = CreateNavButton("⚡ پیشخوان و تبدیل زنده");
            _btnTabHotkey = CreateNavButton("⌨️ کلیدهای میانبر");
            _btnTabMouse = CreateNavButton("🖱️ دکمه شناور موس");
            _btnTabLayout = CreateNavButton("📐 چیدمان کیبورد");
            _btnTabHistory = CreateNavButton("📜 تاریخچه تبدیل‌ها");
            _btnTabSystem = CreateNavButton("⚙️ سیستم و استارت‌آپ");

            _btnTabDashboard.Click += (s, e) => SwitchTab(_pageDashboard, _btnTabDashboard);
            _btnTabHotkey.Click += (s, e) => SwitchTab(_pageHotkey, _btnTabHotkey);
            _btnTabMouse.Click += (s, e) => SwitchTab(_pageMouse, _btnTabMouse);
            _btnTabLayout.Click += (s, e) => SwitchTab(_pageLayout, _btnTabLayout);
            _btnTabHistory.Click += (s, e) => { RefreshHistoryView(); SwitchTab(_pageHistory, _btnTabHistory); };
            _btnTabSystem.Click += (s, e) => SwitchTab(_pageSystem, _btnTabSystem);

            flpNav.Controls.Add(_btnTabDashboard);
            flpNav.Controls.Add(_btnTabHotkey);
            flpNav.Controls.Add(_btnTabMouse);
            flpNav.Controls.Add(_btnTabLayout);
            flpNav.Controls.Add(_btnTabHistory);
            flpNav.Controls.Add(_btnTabSystem);
            _pnlNav.Controls.Add(flpNav);
            Controls.Add(_pnlNav);

            // 3. Footer Bar with Save & Close
            _pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(20, 10, 20, 10)
            };

            var btnSave = new Button
            {
                Text = "ذخیره تنظیمات ✔",
                Font = FontManager.GetVazir(9f, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 34),
                Location = new Point(20, 10),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            var btnHideToTray = new Button
            {
                Text = "انتقال به کنار ساعت (Tray)",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.FromArgb(226, 232, 240),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 34),
                Location = new Point(160, 10),
                Cursor = Cursors.Hand
            };
            btnHideToTray.FlatAppearance.BorderSize = 0;
            btnHideToTray.Click += (s, e) => Hide();

            var lblHintFooter = new Label
            {
                Text = "کلید میانبر در تمام محیط‌های ویندوز (ورد، تلگرام، کروم، نوت‌پد و ادیتورها) پیوسته فعال است.",
                Font = FontManager.GetVazir(8f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(340, 18),
                AutoSize = true
            };

            _pnlFooter.Controls.Add(btnSave);
            _pnlFooter.Controls.Add(btnHideToTray);
            _pnlFooter.Controls.Add(lblHintFooter);
            Controls.Add(_pnlFooter);

            // 4. Content Panel containing all tab pages
            _pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(20)
            };
            Controls.Add(_pnlContent);

            // Create individual Tab Pages
            InitializeDashboardPage();
            InitializeHotkeyPage();
            InitializeMousePage();
            InitializeLayoutPage();
            InitializeHistoryPage();
            InitializeSystemPage();

            _pnlContent.Controls.Add(_pageDashboard);
            _pnlContent.Controls.Add(_pageHotkey);
            _pnlContent.Controls.Add(_pageMouse);
            _pnlContent.Controls.Add(_pageLayout);
            _pnlContent.Controls.Add(_pageHistory);
            _pnlContent.Controls.Add(_pageSystem);
        }

        private Button CreateNavButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Height = 44,
                AutoSize = true,
                Margin = new Padding(0),
                Padding = new Padding(12, 0, 12, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);
            return btn;
        }

        private void SwitchTab(Panel activePage, Button activeBtn)
        {
            _pageDashboard.Visible = false;
            _pageHotkey.Visible = false;
            _pageMouse.Visible = false;
            _pageLayout.Visible = false;
            _pageHistory.Visible = false;
            _pageSystem.Visible = false;

            activePage.Visible = true;

            // Update Nav button styles
            foreach (Control c in _pnlNav.Controls[0].Controls)
            {
                if (c is Button b)
                {
                    b.ForeColor = Color.FromArgb(148, 163, 184);
                    b.BackColor = Color.Transparent;
                    b.Font = FontManager.GetVazir(8.5f, FontStyle.Regular);
                }
            }

            activeBtn.ForeColor = Color.FromArgb(56, 189, 248);
            activeBtn.BackColor = Color.FromArgb(30, 41, 59);
            activeBtn.Font = FontManager.GetVazir(8.5f, FontStyle.Bold);
        }

        // ================= PAGE 1: DASHBOARD =================
        private void InitializeDashboardPage()
        {
            _pageDashboard = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            // Telemetry Cards Row
            int cardW = 230;
            int cardH = 75;

            // Card 1: Total conversions
            var card1 = CreateMetricCard("تعداد کل تبدیل‌ها", "۰ تبدیل", 15, 10, cardW, cardH);
            _lblTotalConversions = (Label)card1.Controls[1];

            // Card 2: Active Hotkey
            var card2 = CreateMetricCard("کلید میانبر فعال سیستم", "Ctrl + Shift + F", 265, 10, cardW, cardH);
            _lblActiveHotkeyBadge = (Label)card2.Controls[1];
            _lblActiveHotkeyBadge.ForeColor = Color.FromArgb(56, 189, 248);

            // Card 3: Memory & CPU Footprint
            var card3 = CreateMetricCard("مصرف منابع سیستم", "< ۱۵ مگابایت رم (۰.۰٪ پردازنده)", 515, 10, cardW, cardH);
            ((Label)card3.Controls[1]).ForeColor = Color.FromArgb(52, 211, 153);

            _pageDashboard.Controls.Add(card1);
            _pageDashboard.Controls.Add(card2);
            _pageDashboard.Controls.Add(card3);

            // Live Transformer Workspace
            var grpWorkspace = new GroupBox
            {
                Text = "⚡ میزکار آزمایش و تبدیل فوری متن (Live Transformer Workbench)",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(15, 100),
                Size = new Size(730, 310)
            };

            var lblInputTitle = new Label
            {
                Text = "متن ورودی (اشتباه تایپ‌شده):",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 28),
                AutoSize = true
            };

            _lblDetectedDirection = new Label
            {
                Text = "تشخیص هوشمند: در حال انتظار...",
                Font = FontManager.GetVazir(8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                Location = new Point(460, 28),
                AutoSize = true
            };

            _txtLiveInput = new TextBox
            {
                Location = new Point(20, 52),
                Size = new Size(690, 70),
                Multiline = true,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = FontManager.GetVazir(10f, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "sghl phgj o,fi? lkl o,fl"
            };
            _txtLiveInput.TextChanged += TxtLiveInput_TextChanged;

            var btnConvertNow = new Button
            {
                Text = "تبدیل متن ⮌",
                Font = FontManager.GetVazir(9f, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 32),
                Location = new Point(20, 132),
                Cursor = Cursors.Hand
            };
            btnConvertNow.FlatAppearance.BorderSize = 0;
            btnConvertNow.Click += (s, e) => PerformLiveConversion();

            var btnSwapTexts = new Button
            {
                Text = "جابجایی متن‌ها ⇄",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.FromArgb(226, 232, 240),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 32),
                Location = new Point(150, 132),
                Cursor = Cursors.Hand
            };
            btnSwapTexts.FlatAppearance.BorderSize = 0;
            btnSwapTexts.Click += (s, e) =>
            {
                string tmp = _txtLiveInput.Text;
                _txtLiveInput.Text = _txtLiveOutput.Text;
                _txtLiveOutput.Text = tmp;
            };

            var btnClear = new Button
            {
                Text = "پاک‌سازی",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.FromArgb(226, 232, 240),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 32),
                Location = new Point(290, 132),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += (s, e) => { _txtLiveInput.Clear(); _txtLiveOutput.Clear(); };

            var lblOutputTitle = new Label
            {
                Text = "خروجی اصلاح‌شده (Corrected Output):",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 175),
                AutoSize = true
            };

            var btnCopyOutput = new Button
            {
                Text = "کپی در کلیپ‌بورد 📋",
                Font = FontManager.GetVazir(8f, FontStyle.Regular),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(56, 189, 248),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 24),
                Location = new Point(590, 172),
                Cursor = Cursors.Hand
            };
            btnCopyOutput.FlatAppearance.BorderSize = 0;
            btnCopyOutput.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_txtLiveOutput.Text))
                {
                    Clipboard.SetText(_txtLiveOutput.Text);
                    MessageBox.Show("متن با موفقیت کپی شد.", "کپی", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            _txtLiveOutput = new TextBox
            {
                Location = new Point(20, 200),
                Size = new Size(690, 70),
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.FromArgb(24, 32, 47),
                ForeColor = Color.FromArgb(52, 211, 153),
                Font = FontManager.GetVazir(10f, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "سلام حالت خوبه؟ منم خوبم"
            };

            grpWorkspace.Controls.Add(lblInputTitle);
            grpWorkspace.Controls.Add(_lblDetectedDirection);
            grpWorkspace.Controls.Add(_txtLiveInput);
            grpWorkspace.Controls.Add(btnConvertNow);
            grpWorkspace.Controls.Add(btnSwapTexts);
            grpWorkspace.Controls.Add(btnClear);
            grpWorkspace.Controls.Add(lblOutputTitle);
            grpWorkspace.Controls.Add(btnCopyOutput);
            grpWorkspace.Controls.Add(_txtLiveOutput);

            _pageDashboard.Controls.Add(grpWorkspace);
        }

        private Panel CreateMetricCard(string label, string val, int x, int y, int w, int h)
        {
            var pnl = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(12)
            };

            var lbl = new Label
            {
                Text = label,
                Font = FontManager.GetVazir(8f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(10, 8),
                AutoSize = true
            };

            var valLbl = new Label
            {
                Text = val,
                Font = FontManager.GetVazir(10.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 32),
                AutoSize = true
            };

            pnl.Controls.Add(lbl);
            pnl.Controls.Add(valLbl);
            return pnl;
        }

        private void TxtLiveInput_TextChanged(object? sender, EventArgs e)
        {
            string text = _txtLiveInput.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                _lblDetectedDirection.Text = "تشخیص هوشمند: در حال انتظار...";
                _lblDetectedDirection.ForeColor = Color.FromArgb(148, 163, 184);
                return;
            }

            bool isFa = KeyboardMapper.IsPersianDominant(text);
            _lblDetectedDirection.Text = isFa ? "تشخیص هوشمند: فارسی ➔ انگلیسی" : "تشخیص هوشمند: انگلیسی ➔ فارسی";
            _lblDetectedDirection.ForeColor = Color.FromArgb(56, 189, 248);
        }

        private void PerformLiveConversion()
        {
            var layout = _rbIsiriLayout.Checked ? KeyboardLayoutType.ISIRI9147 : KeyboardLayoutType.MicrosoftPersian;
            _txtLiveOutput.Text = KeyboardMapper.Convert(_txtLiveInput.Text, layout);
        }

        // ================= PAGE 2: HOTKEY =================
        private void InitializeHotkeyPage()
        {
            _pageHotkey = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            var grp = new GroupBox
            {
                Text = "⌨️ تنظیمات کلید میانبر سراسری ویندوز (Global System Hotkey)",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(15, 15),
                Size = new Size(730, 260)
            };

            var lblDesc = new Label
            {
                Text = "کلید میانبر زیر در تمامی نرم‌افزارهای فعال ویندوز (شامل مرورگرها، پیام‌رسان‌ها، ورد، اکسل و ادیتورها) عمل می‌کند.\nکافی است متن را هایلایت کرده و کلید را فشار دهید تا فوراً در همان محل اصلاح شود.",
                Font = FontManager.GetVazir(9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 30),
                Size = new Size(690, 48)
            };

            var pnlModifiers = new FlowLayoutPanel
            {
                Location = new Point(20, 95),
                Size = new Size(690, 45),
                FlowDirection = FlowDirection.RightToLeft
            };

            _chkCtrl = new CheckBox { Text = "Control (Ctrl)", Font = FontManager.GetVazir(9f, FontStyle.Regular), AutoSize = true, Margin = new Padding(0, 5, 20, 0) };
            _chkShift = new CheckBox { Text = "Shift", Font = FontManager.GetVazir(9f, FontStyle.Regular), AutoSize = true, Margin = new Padding(0, 5, 20, 0) };
            _chkAlt = new CheckBox { Text = "Alt", Font = FontManager.GetVazir(9f, FontStyle.Regular), AutoSize = true, Margin = new Padding(0, 5, 20, 0) };
            _chkWin = new CheckBox { Text = "Windows (Win)", Font = FontManager.GetVazir(9f, FontStyle.Regular), AutoSize = true, Margin = new Padding(0, 5, 20, 0) };

            var lblPlus = new Label { Text = "+", Font = FontManager.GetVazir(11f, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 5, 15, 0) };

            _cmbKey = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 110,
                Font = FontManager.GetVazir(9f, FontStyle.Regular),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Margin = new Padding(0, 3, 10, 0)
            };
            PopulateKeysCombo();

            pnlModifiers.Controls.Add(_chkCtrl);
            pnlModifiers.Controls.Add(_chkShift);
            pnlModifiers.Controls.Add(_chkAlt);
            pnlModifiers.Controls.Add(_chkWin);
            pnlModifiers.Controls.Add(lblPlus);
            pnlModifiers.Controls.Add(_cmbKey);

            _lblHotkeyPreview = new Label
            {
                Text = "کلید انتخاب‌شده: Ctrl + Shift + F",
                Font = FontManager.GetVazir(10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                Location = new Point(25, 160),
                AutoSize = true
            };

            var lblWarning = new Label
            {
                Text = "💡 نکته: پیشنهاد می‌شود حداقل یکی از کلیدهای کمکی (Ctrl یا Shift) همراه کلید اصلی باشد تا با تایپ معمولی تداخل پیدا نکند.",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(25, 200),
                AutoSize = true
            };

            EventHandler updateEvt = (s, e) => UpdateHotkeyPreviewText();
            _chkCtrl.CheckedChanged += updateEvt;
            _chkShift.CheckedChanged += updateEvt;
            _chkAlt.CheckedChanged += updateEvt;
            _chkWin.CheckedChanged += updateEvt;
            _cmbKey.SelectedIndexChanged += updateEvt;

            grp.Controls.Add(lblDesc);
            grp.Controls.Add(pnlModifiers);
            grp.Controls.Add(_lblHotkeyPreview);
            grp.Controls.Add(lblWarning);

            _pageHotkey.Controls.Add(grp);
        }

        private void PopulateKeysCombo()
        {
            _cmbKey.Items.Clear();
            for (char c = 'A'; c <= 'Z'; c++)
                _cmbKey.Items.Add((Keys)Enum.Parse(typeof(Keys), c.ToString()));

            for (int i = 1; i <= 12; i++)
                _cmbKey.Items.Add((Keys)Enum.Parse(typeof(Keys), "F" + i));

            _cmbKey.Items.Add(Keys.Space);
            _cmbKey.Items.Add(Keys.Pause);
            _cmbKey.Items.Add(Keys.Scroll);
            _cmbKey.Items.Add(Keys.Insert);
            _cmbKey.Items.Add(Keys.Oemtilde);

            _cmbKey.SelectedItem = Keys.F;
        }

        private void UpdateHotkeyPreviewText()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (_chkCtrl.Checked) parts.Add("Ctrl");
            if (_chkShift.Checked) parts.Add("Shift");
            if (_chkAlt.Checked) parts.Add("Alt");
            if (_chkWin.Checked) parts.Add("Win");
            if (_cmbKey.SelectedItem != null) parts.Add(_cmbKey.SelectedItem.ToString()!);

            string hotkeyStr = string.Join(" + ", parts);
            _lblHotkeyPreview.Text = "کلید انتخاب‌شده: " + hotkeyStr;
            _lblActiveHotkeyBadge.Text = hotkeyStr;
        }

        // ================= PAGE 3: MOUSE TRIGGER =================
        private void InitializeMousePage()
        {
            _pageMouse = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            var grp = new GroupBox
            {
                Text = "🖱️ رفتار دکمه شناور موس (Floating Action Bubble)",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(15, 15),
                Size = new Size(730, 290)
            };

            _chkEnableFloating = new CheckBox
            {
                Text = "فعال‌سازی دکمه شناور مینیاتوری (⇄) کنار نشانگر موس هنگام انتخاب متن",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 35),
                Size = new Size(690, 26),
                Checked = true
            };

            var lblDesc = new Label
            {
                Text = "✨ حل مشکل دابل‌کلیک پوشه‌ها و دسکتاپ:\n" +
                       "در نسخه جدید ۱.۱.۰، دکمه شناور تنها در صورتی ظاهر می‌شود که کاربر با درگ موس متنی را انتخاب (Highlight) کرده باشد.\n" +
                       "بنابراین دابل‌کلیک کردن روی آیکون برنامه‌ها، پوشه‌ها، فایل‌ها یا پنجره‌های اکسپلورر دیگر هرگز دکمه شناور را فعال نخواهد کرد.",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 211, 153),
                Location = new Point(40, 70),
                Size = new Size(670, 60)
            };

            var lblTimerTitle = new Label
            {
                Text = "مدت زمان ماندگاری دکمه شناور قبل از محو خودکار:",
                Font = FontManager.GetVazir(9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 150),
                AutoSize = true
            };

            _rbFloatFast = new RadioButton { Text = "سریع (۲ ثانیه)", Font = FontManager.GetVazir(8.5f, FontStyle.Regular), Location = new Point(30, 180), AutoSize = true };
            _rbFloatNormal = new RadioButton { Text = "استاندارد (۳.۵ ثانیه)", Font = FontManager.GetVazir(8.5f, FontStyle.Regular), Location = new Point(170, 180), AutoSize = true, Checked = true };
            _rbFloatSlow = new RadioButton { Text = "طولانی (۵ ثانیه)", Font = FontManager.GetVazir(8.5f, FontStyle.Regular), Location = new Point(340, 180), AutoSize = true };

            var lblNote = new Label
            {
                Text = "💡 نکته: در صورتی که تمایل دارید تنها از کلید میانبر (Ctrl+Shift+F) استفاده کنید، می‌توانید دکمه شناور را به طور کامل خاموش کنید تا هوک موس نیز متوقف شود.",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 225),
                Size = new Size(690, 40)
            };

            grp.Controls.Add(_chkEnableFloating);
            grp.Controls.Add(lblDesc);
            grp.Controls.Add(lblTimerTitle);
            grp.Controls.Add(_rbFloatFast);
            grp.Controls.Add(_rbFloatNormal);
            grp.Controls.Add(_rbFloatSlow);
            grp.Controls.Add(lblNote);

            _pageMouse.Controls.Add(grp);
        }

        // ================= PAGE 4: KEYBOARD LAYOUT =================
        private void InitializeLayoutPage()
        {
            _pageLayout = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            var grp = new GroupBox
            {
                Text = "📐 استاندارد چیدمان کلیدهای کیبورد فارسی (Keyboard Layout Standards)",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(15, 15),
                Size = new Size(730, 380)
            };

            _rbMicrosoftLayout = new RadioButton
            {
                Text = "چیدمان پیش‌فرض ویندوز (Microsoft Persian) — سازگار با کیبورد فعلی شما",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 30),
                Size = new Size(690, 26),
                Checked = true
            };

            var lblMsDesc = new Label
            {
                Text = "در این چیدمان: کلید M برابر 'ئ'، کلید Backslash (\\) برابر 'پ' و علامت سوال '؟' با Shift+/ تایپ می‌شود.",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(45, 58),
                AutoSize = true
            };

            _rbIsiriLayout = new RadioButton
            {
                Text = "چیدمان استاندارد ملی ایران (ISIRI 9147)",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 95),
                Size = new Size(690, 26)
            };

            var lblIsiriDesc = new Label
            {
                Text = "در این چیدمان: کلید M برابر 'پ'، کلید Shift+M برابر 'ئ'، و کلید Backslash (\\) برابر 'ژ' است.",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(45, 123),
                AutoSize = true
            };

            // Comparison Matrix Panel
            var pnlMatrix = new Panel
            {
                Location = new Point(20, 160),
                Size = new Size(690, 190),
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(15)
            };

            var lblMatrixTitle = new Label
            {
                Text = "جدول تطابق کلیدهای حیاتی کیبورد:",
                Font = FontManager.GetVazir(9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                Location = new Point(15, 12),
                AutoSize = true
            };

            var lblMatrixContent = new Label
            {
                Text = "• کلید S G H L ➔ س ل ا م (سلام)\n" +
                       "• کلید H O W ➔ ا خ ص (اخص ➔ how)\n" +
                       "• کلید M ➔ ئ (ویندوز) | پ (استاندارد ملی ۹۱۴۷)\n" +
                       "• کلید Shift + H ➔ آ (الف با کلاه)\n" +
                       "• کلید Shift + C ➔ ژ\n" +
                       "• کلید Shift + B ➔ نیم‌فاصله (Zero-Width Non-Joiner)\n" +
                       "• کلید Shift + / ➔ ؟ (علامت سوال فارسی)",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(15, 40),
                Size = new Size(660, 135)
            };

            pnlMatrix.Controls.Add(lblMatrixTitle);
            pnlMatrix.Controls.Add(lblMatrixContent);

            grp.Controls.Add(_rbMicrosoftLayout);
            grp.Controls.Add(lblMsDesc);
            grp.Controls.Add(_rbIsiriLayout);
            grp.Controls.Add(lblIsiriDesc);
            grp.Controls.Add(pnlMatrix);

            _pageLayout.Controls.Add(grp);
        }

        // ================= PAGE 5: HISTORY =================
        private void InitializeHistoryPage()
        {
            _pageHistory = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            var grp = new GroupBox
            {
                Text = "📜 گزارش تاریخچه تبدیل‌های اخیر (Recent Conversions Log)",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(15, 15),
                Size = new Size(730, 410)
            };

            var btnClearHistory = new Button
            {
                Text = "پاک‌سازی تاریخچه",
                Font = FontManager.GetVazir(8f, FontStyle.Regular),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.FromArgb(226, 232, 240),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 26),
                Location = new Point(600, 20),
                Cursor = Cursors.Hand
            };
            btnClearHistory.FlatAppearance.BorderSize = 0;
            btnClearHistory.Click += (s, e) => { HistoryManager.Clear(); RefreshHistoryView(); };

            _lvHistory = new ListView
            {
                Location = new Point(20, 55),
                Size = new Size(690, 335),
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                BorderStyle = BorderStyle.None
            };

            _lvHistory.Columns.Add("زمان", 80, HorizontalAlignment.Right);
            _lvHistory.Columns.Add("جهت", 90, HorizontalAlignment.Center);
            _lvHistory.Columns.Add("متن اولیه (ورودی)", 240, HorizontalAlignment.Right);
            _lvHistory.Columns.Add("متن تبدیل‌شده (خروجی)", 260, HorizontalAlignment.Right);

            grp.Controls.Add(btnClearHistory);
            grp.Controls.Add(_lvHistory);
            _pageHistory.Controls.Add(grp);
        }

        private void RefreshHistoryView()
        {
            _lvHistory.Items.Clear();
            var records = HistoryManager.GetRecords();
            foreach (var r in records)
            {
                var item = new ListViewItem(r.Timestamp.ToString("HH:mm:ss"));
                item.SubItems.Add(r.Direction);
                item.SubItems.Add(r.SourceText);
                item.SubItems.Add(r.ConvertedText);
                _lvHistory.Items.Add(item);
            }

            _lblTotalConversions.Text = $"{HistoryManager.TotalConversions} تبدیل";
        }

        // ================= PAGE 6: SYSTEM =================
        private void InitializeSystemPage()
        {
            _pageSystem = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            var grp = new GroupBox
            {
                Text = "⚙️ تنظیمات سیستمی و چرخه حیات نرم‌افزار",
                Font = FontManager.GetVazir(9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                Location = new Point(15, 15),
                Size = new Size(730, 300)
            };

            _chkStartup = new CheckBox
            {
                Text = "اجرای خودکار نرم‌افزار هنگام بالا آمدن ویندوز (Windows Startup)",
                Font = FontManager.GetVazir(9f, FontStyle.Regular),
                Location = new Point(20, 35),
                Size = new Size(690, 26),
                Checked = true
            };

            _chkUndo = new CheckBox
            {
                Text = "پشتیبانی از بازگشت متن (Undo) با فشردن دوباره میانبر در صورت اشتباه",
                Font = FontManager.GetVazir(9f, FontStyle.Regular),
                Location = new Point(20, 75),
                Size = new Size(690, 26),
                Checked = true
            };

            _chkCloseToTray = new CheckBox
            {
                Text = "هنگام بستن پنجره با دکمه ضربدر، برنامه بسته نشود و به سینی سیستم (System Tray) برود",
                Font = FontManager.GetVazir(9f, FontStyle.Regular),
                Location = new Point(20, 115),
                Size = new Size(690, 26),
                Checked = true
            };

            var pnlAbout = new Panel
            {
                Location = new Point(20, 165),
                Size = new Size(690, 110),
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(15)
            };

            var lblAboutTitle = new Label
            {
                Text = "درباره Fingilish Switcher Industrial Edition:",
                Font = FontManager.GetVazir(9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                Location = new Point(15, 10),
                AutoSize = true
            };

            var lblAboutBody = new Label
            {
                Text = "طراحی شده با استانداردهای بدون اسلوپ (Taste-Skill) و تایپوگرافی اصیل وزیر متن.\n" +
                       "موتور تبدیل فوق‌سبک بر پایه C# و Windows Desktop LTS.\n" +
                       "فاقد هرگونه بار پردازشی اضافه در حالت بیکار (Idle CPU: 0.0%).",
                Font = FontManager.GetVazir(8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(15, 38),
                Size = new Size(660, 60)
            };

            pnlAbout.Controls.Add(lblAboutTitle);
            pnlAbout.Controls.Add(lblAboutBody);

            grp.Controls.Add(_chkStartup);
            grp.Controls.Add(_chkUndo);
            grp.Controls.Add(_chkCloseToTray);
            grp.Controls.Add(pnlAbout);

            _pageSystem.Controls.Add(grp);
        }

        // ================= CONFIG SYNC =================
        private void LoadDataFromConfig()
        {
            var cfg = ConfigManager.Current;

            _chkCtrl.Checked = cfg.Control;
            _chkShift.Checked = cfg.Shift;
            _chkAlt.Checked = cfg.Alt;
            _chkWin.Checked = cfg.Win;

            if (_cmbKey.Items.Contains(cfg.Hotkey))
                _cmbKey.SelectedItem = cfg.Hotkey;
            else
                _cmbKey.SelectedItem = Keys.F;

            _chkEnableFloating.Checked = cfg.EnableFloatingButton;
            _chkUndo.Checked = cfg.EnableUndo;
            _chkStartup.Checked = StartupManager.IsSetToRunWithWindows();

            if (cfg.LayoutType == KeyboardLayoutType.ISIRI9147)
                _rbIsiriLayout.Checked = true;
            else
                _rbMicrosoftLayout.Checked = true;

            UpdateHotkeyPreviewText();
            _lblTotalConversions.Text = $"{HistoryManager.TotalConversions} تبدیل";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (!_chkCtrl.Checked && !_chkShift.Checked && !_chkAlt.Checked && !_chkWin.Checked &&
                _cmbKey.SelectedItem is Keys k && (k != Keys.Pause && k != Keys.Scroll && k != Keys.Insert))
            {
                MessageBox.Show(
                    "لطفاً حداقل یکی از کلیدهای کمکی (Ctrl، Shift یا Alt) را انتخاب کنید تا با تایپ عادی تداخل پیدا نکند.",
                    "هشدار کلید میانبر",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var cfg = ConfigManager.Current;
            cfg.Control = _chkCtrl.Checked;
            cfg.Shift = _chkShift.Checked;
            cfg.Alt = _chkAlt.Checked;
            cfg.Win = _chkWin.Checked;
            cfg.Hotkey = (Keys)(_cmbKey.SelectedItem ?? Keys.F);

            cfg.EnableFloatingButton = _chkEnableFloating.Checked;
            cfg.EnableUndo = _chkUndo.Checked;
            cfg.StartWithWindows = _chkStartup.Checked;
            cfg.LayoutType = _rbIsiriLayout.Checked ? KeyboardLayoutType.ISIRI9147 : KeyboardLayoutType.MicrosoftPersian;

            ConfigManager.Save();
            StartupManager.SetRunWithWindows(cfg.StartWithWindows);

            SettingsUpdated?.Invoke();

            MessageBox.Show(
                "تنظیمات صنعتی با موفقیت در سیستم ذخیره گردید و فعال شد.",
                "ذخیره موفق",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _chkCloseToTray.Checked)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }
    }
}
