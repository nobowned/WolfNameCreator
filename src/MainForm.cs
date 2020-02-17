using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace WolfNameCreator
{
    public partial class MainForm : Form
    {
        List<SelectablePictureBox> PictureBoxes;
        List<Image> WolfFont;
        WolfTextField TextField;

        public MainForm()
        {
            InitializeComponent();
            InitializeSelectablePictureBoxes();

            TextField = new WolfTextField(this, new Point(12, 50), 35, WolfFont);
            TextField.Parent = this;
            Controls.Add(TextField);

            Width = TextField.Width + 40;
            Height = 120;
            CopyButton.Location = new Point(Width - CopyButton.Width - 28, 6);
            SaveButton.Location = new Point(Width - CopyButton.Width - SaveButton.Width - 28, 6);

            TextField.TabIndex = 0;
            CopyButton.TabIndex = 1;
        }

        void InitializeSelectablePictureBoxes()
        {
            PictureBoxes = new List<SelectablePictureBox>();
            int LocationX = 12;
            int LocationY = 12;

            // Initialize the special character picture boxes.
            // 0-32 are the control characters used to encode the special characters in RTCW.
            int ActuallyUsed = 0;
            for (int i = 0; i < 32; ++i)
            {
                if (WolfNameHelper.IsInvalidCodepoint(i))
                {
                    continue;
                }

                PictureBoxes.Add(new SelectablePictureBox(i, PicBox_MouseDown)
                {
                    Image = Properties.Resources.WolfChars.Clone(WolfNameHelper.GetCodepointBounds(i), Properties.Resources.WolfChars.PixelFormat),
                    Location = new Point(LocationX + WolfNameHelper.ImageWidth * ActuallyUsed++, LocationY),
                    Width = WolfNameHelper.ImageWidth,
                    Height = WolfNameHelper.ImageHeight,
                    Parent = this
                });
            }

            // Initialize the color picture boxes.
            // Codepoints 32-39 are reserved for the colors, they aren't really codepoints.
            LocationX = 12;
            LocationY = 28;
            ActuallyUsed = 0;
            for (int i = 32; i < 32 + 8; ++i)
            {
                var ColorIndex = i % WolfNameHelper.ImageWidth;
                PictureBoxes.Add(new SelectablePictureBox(i, PicBox_MouseDown)
                {
                    Location = new Point(LocationX + 18 * ActuallyUsed++, LocationY),
                    Width = WolfNameHelper.ImageWidth,
                    Height = WolfNameHelper.ImageHeight,
                    Parent = this,
                    BackColor = WolfColorUtil.WolfColorToRealColor(ColorIndex),
                    BorderStyle = BorderStyle.FixedSingle
                });
            }

            // Create the "WolfFont"; which is just a list of bitmaps, each bitmap a wolf character.
            WolfFont = new List<Image>();
            for (int i = 0; i <= WolfNameHelper.MaxCodepoint; ++i)
            {
                WolfFont.Add(Properties.Resources.WolfChars.Clone(WolfNameHelper.GetCodepointBounds(i), Properties.Resources.WolfChars.PixelFormat));
            }
        }

        private void PicBox_MouseDown(int codepoint)
        {
            TextField.Focus();

            if (codepoint > 31)
            {
                var Color = WolfColorUtil.CodepointToRealColor(codepoint);
                var NumberCodepoint = (codepoint % WolfNameHelper.ImageWidth) + 48;
                TextField.AddColorCharacter(Color, NumberCodepoint);
            }
            else
            {
                TextField.AddSpecialCharacter(codepoint);
            }
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            var Text = TextField.GetText();
            Clipboard.SetText(string.IsNullOrEmpty(Text) ? " " : Text);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var FileDialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                DefaultExt = ".cfg"
            };

            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var Stream = FileDialog.OpenFile())
                {
                    using (var Writer = new StreamWriter(Stream))
                    {
                        Writer.WriteLine(string.Format("set name \"{0}\"", TextField.GetText()));
                    }
                }
            }
        }
    }
}
