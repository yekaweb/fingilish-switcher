using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FingilishSwitcher
{
    public class FloatingBubbleForm : Form
    {
        private readonly Action<IntPtr> _onTriggerConvert;
        private readonly System.Windows.Forms.Timer _autoHideTimer;
        private bool _isHovered = false;
        private IntPtr _targetWindow = IntPtr.Zero;

        public FloatingBubbleForm(Action<IntPtr> onTriggerConvert)
        {
            _onTriggerConvert = onTriggerConvert;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(34, 30);
            BackColor = Color.FromArgb(15, 23, 42); // Deep slate
            Cursor = Cursors.Hand;
            DoubleBuffered = true;

            _autoHideTimer = new System.Windows.Forms.Timer { Interval = 3200 };
            _autoHideTimer.Tick += (s, e) =>
            {
                _autoHideTimer.Stop();
                Hide();
            };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // WS_EX_TOOLWINDOW (does not show in Alt-Tab)
                // WS_EX_NOACTIVATE (prevent stealing focus)
                // WS_EX_TOPMOST
                cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE | NativeMethods.WS_EX_TOPMOST;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // CRITICAL: Intercept WM_MOUSEACTIVATE to completely prevent Windows from activating this window on click!
            if (m.Msg == NativeMethods.WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }

        public void ShowAt(int x, int y, IntPtr targetWindow)
        {
            _targetWindow = targetWindow;

            // Keep within screen bounds
            var screen = Screen.FromPoint(new Point(x, y)).WorkingArea;
            int posX = Math.Min(x, screen.Right - Width - 10);
            int posY = Math.Max(screen.Top + 10, Math.Min(y, screen.Bottom - Height - 10));

            Location = new Point(posX, posY);
            if (!Visible)
                Show();

            _autoHideTimer.Stop();
            _autoHideTimer.Start();
            Invalidate();
        }

        public new void Hide()
        {
            _autoHideTimer.Stop();
            base.Hide();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            _autoHideTimer.Stop();
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            _autoHideTimer.Start();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                IntPtr target = _targetWindow;
                Hide();
                _onTriggerConvert?.Invoke(target);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Rounded rectangle
            var rect = new Rectangle(1, 1, Width - 3, Height - 3);

            Color bg = _isHovered ? Color.FromArgb(30, 41, 59) : Color.FromArgb(15, 23, 42);
            using (var brush = new SolidBrush(bg))
            {
                g.FillRectangle(brush, rect);
            }

            // 1px subtle technical border
            Color borderColor = _isHovered ? Color.FromArgb(56, 189, 248) : Color.FromArgb(51, 65, 85);
            using (var pen = new Pen(borderColor, 1.2f))
            {
                g.DrawRectangle(pen, rect);
            }

            // Draw technical swap icon
            using (var font = new Font("Segoe UI", 10f, FontStyle.Bold))
            using (var brush = new SolidBrush(_isHovered ? Color.FromArgb(56, 189, 248) : Color.FromArgb(226, 232, 240)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("⇄", font, brush, new RectangleF(0, 0, Width, Height), sf);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoHideTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
