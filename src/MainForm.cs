using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WolfNameCreator.Properties;

namespace WolfNameCreator
{
    public partial class MainForm : Form
    {
        const string WolfConfigExtension = ".cfg";
        const string NameKey = " name ";

        List<SelectablePictureBox> PictureBoxes;
        List<Image> WolfFont;
        WolfTextField TextField;
        string ConfigFilePath;

        public MainForm(string[] args)
        {
            InitializeComponent();
            InitializeSelectablePictureBoxes();

            TextField = new WolfTextField(this, new Point(12, MenuStrip.Height + 50), 35, WolfFont)
            {
                Parent = this,
                OnCtrlSKeyed = () =>
                {
                    if (SaveToolStripMenuItem.Enabled)
                    {
                        SaveToolStripMenuItem.PerformClick();
                    }
                    else
                    {
                        SaveNewToolStripMenuItem.PerformClick();
                    }
                }
            };
            Controls.Add(TextField);
            TextField.TabIndex = 0;

            Width = TextField.Width + 40;
            Height = 120 + MenuStrip.Height;

            ParseArguments(args);

            SaveToolStripMenuItem.Enabled = !string.IsNullOrEmpty(ConfigFilePath);

            DrawColorCodesCheckBox.CheckedChanged -= DrawColorCodesCheckBox_CheckedChanged;
            DrawColorCodesCheckBox.Checked = TextField.DrawColorCodes;
            DrawColorCodesCheckBox.CheckedChanged += DrawColorCodesCheckBox_CheckedChanged;
        }

        void InitializeSelectablePictureBoxes()
        {
            PictureBoxes = new List<SelectablePictureBox>();
            int LocationX = 12;
            int LocationY = MenuStrip.Height + 12;

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
            LocationY = MenuStrip.Height + 28;
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
                    BorderPen = new Pen(Color.Black)
                });
            }

            // Create the "WolfFont"; which is just a list of bitmaps, each bitmap a wolf character.
            WolfFont = new List<Image>();
            for (int i = 0; i <= WolfNameHelper.MaxCodepoint; ++i)
            {
                WolfFont.Add(Properties.Resources.WolfChars.Clone(WolfNameHelper.GetCodepointBounds(i), Properties.Resources.WolfChars.PixelFormat));
            }
        }

        void ParseArguments(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                return;
            }

            var FirstArgument = args[0];
            if (Path.GetExtension(FirstArgument) == WolfConfigExtension)
            {
                if (File.Exists(FirstArgument))
                {
                    var PlayerName = ParsePlayerNameFromConfigFile(FirstArgument);
                    if (!string.IsNullOrEmpty(PlayerName))
                    {
                        ConfigFilePath = FirstArgument;
                        TextField.SetText(PlayerName);
                    }
                }
            }
            else
            {
                TextField.SetText(FirstArgument);
            }
        }

        string ParsePlayerNameFromConfigFile(string configFilePath)
        {
            var ConfigFileContents = File.ReadAllLines(configFilePath);
            if (ConfigFileContents.Length == 0)
            {
                return null;
            }

            var SetNameLine = ConfigFileContents.FirstOrDefault(line => line.Contains(NameKey));
            if (!string.IsNullOrEmpty(SetNameLine))
            {
                var Name = SetNameLine.Substring(SetNameLine.IndexOf(NameKey) + (NameKey.Length - 1)).Trim(' ', '\n', '\r', '"');
                return Name;
            }

            return null;
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

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var FileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                CheckFileExists = true
            };

            using (FileDialog)
            {
                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    var PlayerName = ParsePlayerNameFromConfigFile(FileDialog.FileName);
                    if (!string.IsNullOrEmpty(PlayerName))
                    {
                        TextField.SetText(PlayerName);
                        ConfigFilePath = FileDialog.FileName;
                        SaveToolStripMenuItem.Enabled = !string.IsNullOrEmpty(ConfigFilePath);
                    }
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConfigFilePath) || !File.Exists(ConfigFilePath))
            {
                return;
            }

            Cursor = Cursors.WaitCursor;

            var Lines = File.ReadAllLines(ConfigFilePath).ToList();
            Lines.RemoveAll(line => line.Contains(NameKey));
            Lines.Add($"set name \"{TextField.GetFullText()}\"");
            File.WriteAllLines(ConfigFilePath, Lines.ToArray());

            Task.Delay(100).ContinueWith(t => Cursor = Cursors.Default, scheduler: TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SaveNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var FileDialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                DefaultExt = WolfConfigExtension
            };

            using (FileDialog)
            {
                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var Stream = FileDialog.OpenFile())
                    {
                        using (var Writer = new StreamWriter(Stream))
                        {
                            Writer.WriteLine($"set name \"{TextField.GetFullText()}\"");
                            ConfigFilePath = FileDialog.FileName;
                            SaveToolStripMenuItem.Enabled = !string.IsNullOrEmpty(ConfigFilePath);
                        }
                    }
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.FormLocation = Location;
            Settings.Default.FormLocationSaved = true;
            Settings.Default.Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Settings.Default.FormLocationSaved)
            {
                Location = Settings.Default.FormLocation;
            }
        }

        private void DrawColorCodesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TextField.ToggleDrawColorCodes();
            TextField.Focus();
        }
    }
}
