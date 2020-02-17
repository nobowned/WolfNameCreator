using System;
using System.Drawing;
using System.Windows.Forms;

namespace WolfNameCreator
{
    public class SelectablePictureBox : PictureBox
    {
        int Codepoint;
        bool Clicked;
        Timer HighlightTimer;
        Action<int> MouseDownEvent;

        public SelectablePictureBox(int codepoint, Action<int> mouseDown)
        {
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
            Clicked = false;
            HighlightTimer = new Timer();
            HighlightTimer.Tick += (sender, e) =>
            {
                Clicked = false;
                HighlightTimer.Stop();
                Refresh();
            };
            HighlightTimer.Interval = 100;
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
                if (!HighlightTimer.Enabled)
                {
                    HighlightTimer.Start();
                }
            }
        }
    }
}
