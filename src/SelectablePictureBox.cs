using System;
using System.Drawing;
using System.Windows.Forms;

namespace WolfNameCreator
{
    public class SelectablePictureBox : PictureBox
    {
        int Codepoint;
        bool Clicked;
        Timer SelectedTimer;
        Action<int> MouseDownEvent;
        Pen PreviousBorderPen;
        Rectangle? _borderBounds;
        Rectangle BorderBounds
        {
            get
            {
                if (_borderBounds == null)
                {
                    _borderBounds = new Rectangle(0, 0, Width - 1, Height - 1);
                }

                return _borderBounds.Value;
            }
        }

        public Pen BorderPen;

        readonly static Pen HighlightBorderPen = new Pen(Color.DeepSkyBlue);

        public SelectablePictureBox(int codepoint, Action<int> mouseDown)
        {
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
            Clicked = false;
            SelectedTimer = new Timer();
            SelectedTimer.Tick += (sender, e) =>
            {
                Clicked = false;
                SelectedTimer.Stop();
                Refresh();
            };
            SelectedTimer.Interval = 100;
            MouseDownEvent = mouseDown;
            Codepoint = codepoint;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Clicked = true;
            Refresh();
            MouseDownEvent(Codepoint);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Clicked)
            {
                e.Graphics.Clear(Color.Cornsilk);
                if (!SelectedTimer.Enabled)
                {
                    SelectedTimer.Start();
                }
            }
            if (BorderPen != null)
            {
                e.Graphics.DrawRectangle(BorderPen, BorderBounds);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            PreviousBorderPen = BorderPen;
            BorderPen = HighlightBorderPen;
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            BorderPen = PreviousBorderPen;
            Refresh();
        }
    }
}
