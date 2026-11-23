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
            Size = new Size(36, 30);
            BackColor = Color.FromArgb(15, 23, 42); // Deep slate #0F172A
            Cursor = Cursors.Hand;
            DoubleBuffered = true;

            // Force native handle creation on the UI thread
            var _ = Handle;

            _autoHideTimer = new System.Windows.Forms.Timer { Interval = 3500 };
            _autoHideTimer.Tick += (s, e) =>
            {
                _autoHideTimer.Stop();
                HideBubble();
            };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // WS_EX_TOOLWINDOW: Hide from Alt-Tab
                // WS_EX_NOACTIVATE: Never steal focus from active text editor
                // WS_EX_TOPMOST: Stay on top of other windows
                cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE | NativeMethods.WS_EX_TOPMOST;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Intercept WM_MOUSEACTIVATE to strictly prevent Windows from activating this window on click!
            if (m.Msg == NativeMethods.WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }

        public void ShowAt(int x, int y, IntPtr targetWindow)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowAt(x, y, targetWindow)));
                return;
            }

            _targetWindow = targetWindow;

            // Keep within current monitor working area
            var screen = Screen.FromPoint(new Point(x, y)).WorkingArea;
            int posX = Math.Max(screen.Left + 5, Math.Min(x, screen.Right - Width - 10));
            int posY = Math.Max(screen.Top + 5, Math.Min(y, screen.Bottom - Height - 10));

            // Set position and show without stealing focus
            NativeMethods.SetWindowPos(
                Handle,
                NativeMethods.HWND_TOPMOST,
                posX,
                posY,
                Width,
                Height,
                NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW
            );
            NativeMethods.ShowWindow(Handle, NativeMethods.SW_SHOWNOACTIVATE);
            Visible = true;

            _autoHideTimer.Stop();
            _autoHideTimer.Start();
            Invalidate();
        }

        public void HideBubble()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HideBubble));
                return;
            }

            _autoHideTimer.Stop();
            NativeMethods.ShowWindow(Handle, NativeMethods.SW_HIDE);
            Visible = false;
        }

        public new void Hide()
        {
            HideBubble();
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
                HideBubble();
                _onTriggerConvert?.Invoke(target);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Rounded rectangle container
            var rect = new Rectangle(1, 1, Width - 3, Height - 3);

            Color bg = _isHovered ? Color.FromArgb(30, 41, 59) : Color.FromArgb(15, 23, 42);
            using (var brush = new SolidBrush(bg))
            {
                g.FillRectangle(brush, rect);
            }

            // High-contrast clean border
            Color borderColor = _isHovered ? Color.FromArgb(56, 189, 248) : Color.FromArgb(71, 85, 105);
            using (var pen = new Pen(borderColor, 1.2f))
            {
                g.DrawRectangle(pen, rect);
            }

            // Draw technical swap icon
            using (var font = new Font("Segoe UI", 11f, FontStyle.Bold))
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
