using System;
using System.Collections.Generic;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace WolfNameCreator
{
    public class WolfTextField : Control
    {
        class Command
        {
            public enum Type
            {
                DeleteSelectedCharacter,
                DeletePreviousCharacter,
                AddNormalCharacter,
                AddColorCharacter,
                AddSpecialCharacter,
                Clear,
                CopyToClipboard,
                PasteFromClipboard,
                ToggleDrawColorCodes
            }

            public Type T;
            public List<object> ExecuteArgs;
            public List<object> UndoArgs;
        }

        const string ClearMenuItemName = "Clear";
        const string CopyMenuItemName = "Copy";
        const string PasteMenuItemName = "Paste";

        readonly List<Image> WolfFont;
        readonly int MaxLength;
        readonly Stack<Command> ExecutedCommands;
        readonly Stack<Command> UndoneCommands;
        readonly ContextMenuStrip ActionsMenuStrip;
        readonly List<Character> CharsToDraw;
        readonly Pen FocusedPen;
        readonly Pen UnfocusedPen;
        readonly Pen CursorPen;
        readonly Timer CursorTimer;

        Point StartingCursorPosition;
        Point CursorPosition;
        Size CursorSize;
        bool FocusObtained;
        bool DrawCursor;
        public bool DrawColorCodes { get; private set; } = true;
        List<Character> ColorlessChars
        {
            get
            {
                List<Character> Result = null;
                if (!DrawColorCodes)
                {
                    Result = new List<Character>();
                    for (int i = 0; i < CharsToDraw.Count; ++i)
                    {
                        if (CharsToDraw[i].IsColorEscape && (i + 1) < CharsToDraw.Count)
                        {
                            i++;
                        }
                        else
                        {
                            Result.Add(CharsToDraw[i]);
                        }
                    }
                }
                return Result;
            }
        }

        public Action OnCtrlSKeyed { get; set; }

        public WolfTextField(Control parent, Point loc, int maxLen, List<Image> wolfFont)
            : base(parent, null, loc.X, loc.Y, WolfFontHelper.ImageWidth * maxLen + 2, WolfFontHelper.ImageHeight + 4)
        {
            MaxLength = maxLen;
            BackColor = Color.LightGray;
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
            FocusObtained = false;
            CharsToDraw = new List<Character>();
            FocusedPen = new Pen(Color.Blue);
            UnfocusedPen = new Pen(Color.Black);
            CursorPen = new Pen(Color.Black);
            CursorPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            CursorTimer = new Timer();
            CursorTimer.Tick += (sender, e) =>
            {
                DrawCursor = !DrawCursor;
                InvalidateCursor();
            };
            CursorTimer.Interval = 500;
            CursorTimer.Start();
            DrawCursor = true;
            StartingCursorPosition = new Point(2, 2);
            CursorPosition = StartingCursorPosition;
            CursorSize = new Size(14, 14);
            ActionsMenuStrip = new ContextMenuStrip();
            ActionsMenuStrip.Items.Add(ClearMenuItemName);
            ActionsMenuStrip.Items.Add(CopyMenuItemName);
            ActionsMenuStrip.Items.Add(PasteMenuItemName);
            ActionsMenuStrip.ItemClicked += ActionsMenuStrip_ItemClicked;
            ExecutedCommands = new Stack<Command>();
            UndoneCommands = new Stack<Command>();
            WolfFont = wolfFont;
        }

        void ActionsMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == ClearMenuItemName)
            {
                ClearText();
            }
            else if (e.ClickedItem.Text == CopyMenuItemName)
            {
                CopyToClipboard();
            }
            else if (e.ClickedItem.Text == PasteMenuItemName)
            {
                PasteFromClipboard();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (FocusObtained || Focused)
            {
                e.Graphics.DrawRectangle(FocusedPen, 0, 0, Width - 2, Height - 2);
                FocusObtained = false;
            }
            else
            {
                e.Graphics.DrawRectangle(UnfocusedPen, 0, 0, Width - 2, Height - 2);
            }

            int ImgIndex = 0;
            foreach (var Char in (DrawColorCodes ? CharsToDraw : ColorlessChars))
            {
                e.Graphics.DrawImage(Char.Img, new Rectangle((ImgIndex++ * WolfFontHelper.ImageWidth) + 1, 1,
                    Char.Img.Width, Char.Img.Height), 0, 0, Char.Img.Width, Char.Img.Height,
                    GraphicsUnit.Pixel, Char.ImageAttributes);
            }

            if (DrawCursor)
            {
                e.Graphics.DrawRectangle(CursorPen, new Rectangle(CursorPosition, CursorSize));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // ctrl + left (move cursor to end)
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Left)
            {
                InvalidateCursor();
                CursorPosition = StartingCursorPosition;
                InvalidateCursor();
            }
            // ctrl + right (move cursor to beginning)
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Right)
            {
                InvalidateCursor();
                CursorPosition.X = CharsToDraw.Count * 16 + StartingCursorPosition.X;
                InvalidateCursor();
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Focus();

            if (e.Button == MouseButtons.Right)
            {
                ActionsMenuStrip.Items[0].Enabled = ActionsMenuStrip.Items[1].Enabled = CharsToDraw.Count > 0;
                ActionsMenuStrip.Show(this, e.Location);
            }
            else
            {
                var Position = e.X / 16;
                if (Position > CharsToDraw.Count)
                {
                    Position = CharsToDraw.Count;
                }

                InvalidateCursor();
                CursorPosition.X = Position * 16 + 2;
                InvalidateCursor();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            FocusObtained = true;
            DrawCursor = true;
            CursorTimer.Start();
            Invalidate(new Rectangle(0, 0, Width, 1));
            Invalidate(new Rectangle(0, 0, 1, Height));
            Invalidate(new Rectangle(Width - 2, 0, 1, Height));
            Invalidate(new Rectangle(0, Height - 2, Width, 1));
            Update();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            DrawCursor = false;
            CursorTimer.Stop();
            Invalidate(new Rectangle(0, 0, Width, 1));
            Invalidate(new Rectangle(0, 0, 1, Height));
            Invalidate(new Rectangle(Width - 2, 0, 1, Height));
            Invalidate(new Rectangle(0, Height - 2, Width, 1));
            Invalidate(new Rectangle(CursorPosition, WolfFontHelper.GetCodepointSize()));
            DrawCursor = false;
            Update();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            var Key = char.ConvertToUtf32(e.KeyChar.ToString(), 0);
            HandleKeyPressed(Key);
        }

        public void HandleKeyPressed(int key)
        {
            if (key == (int)Keys.Back)
            {
                var PreviousXPosition = CursorPosition.X;
                var DeleteResult = DeletePreviousCharacter();
                if (DeleteResult.Item1 != -1)
                {
                    PushUndoableCommand(new Command
                    {
                        T = Command.Type.DeletePreviousCharacter,
                        ExecuteArgs = new List<object> { PreviousXPosition },
                        UndoArgs = new List<object> { DeleteResult.Item1, DeleteResult.Item2 }
                    });
                }
            }

            int Codepoint = key;
            if (Codepoint >= 32 && Codepoint <= WolfFontHelper.MaxCodepoint)
            {
                var CursorXPositionBeforeAdd = CursorPosition.X;
                AddNormalCharacter(Codepoint);
                PushUndoableCommand(new Command
                {
                    T = Command.Type.AddNormalCharacter,
                    ExecuteArgs = new List<object> { Codepoint, CursorXPositionBeforeAdd },
                    UndoArgs = new List<object> { CursorPosition.X }
                });
            }
        }

        public string GetText()
        {
            var Result = string.Empty;
            foreach (var Char in CharsToDraw)
            {
                Result += char.ConvertFromUtf32(Char.Codepoint);
            }
            return Result;
        }

        public void SetText(string text)
        {
            CursorPosition = StartingCursorPosition;
            CharsToDraw.Clear();
            Invalidate();
            Update();

            text = text ?? string.Empty;
            foreach (var Char in text)
            {
                AddNormalCharacter(char.ConvertToUtf32(Char.ToString(), 0));
            }
        }

        public void AppendText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            foreach (var Char in text)
            {
                AddNormalCharacter(char.ConvertToUtf32(Char.ToString(), 0));
            }
        }

        public void ToggleDrawColorCodes(bool fromUndoRedoSystem = false)
        {
            var PreviousCharacters = new List<Character>(CharsToDraw);
            var PreviousXPosition = CursorPosition.X;
            DrawColorCodes = !DrawColorCodes;
            Enabled = DrawColorCodes;
            Focus();
            Refresh();
            if (!fromUndoRedoSystem)
            {
                PushUndoableCommand(new Command
                {
                    T = Command.Type.ToggleDrawColorCodes,
                    ExecuteArgs = new List<object> { DrawColorCodes, CharsToDraw.ToList(), CursorPosition.X },
                    UndoArgs = new List<object> { !DrawColorCodes, PreviousCharacters, PreviousXPosition }
                });
            }
        }

        public void Undo()
        {
            if (ExecutedCommands.Count > 0)
            {
                var Cmd = ExecutedCommands.Pop();
                UndoCommand(Cmd);
                UndoneCommands.Push(Cmd);
            }
        }

        public void Redo()
        {
            if (UndoneCommands.Count > 0)
            {
                var Cmd = UndoneCommands.Pop();
                ExecuteCommand(Cmd);
                ExecutedCommands.Push(Cmd);
            }
        }

        public void ClearText()
        {
            var PreviousText = GetText();
            Clear();
            PushUndoableCommand(new Command
            {
                T = Command.Type.Clear,
                ExecuteArgs = new List<object> { },
                UndoArgs = new List<object> { PreviousText }
            });
        }

        public void PasteFromClipboard()
        {
            var PreviousText = GetText();
            AppendText(Clipboard.GetText());
            PushUndoableCommand(new Command
            {
                T = Command.Type.PasteFromClipboard,
                ExecuteArgs = new List<object> { Clipboard.GetText() },
                UndoArgs = new List<object> { PreviousText }
            });
        }

        public void CopyToClipboard()
        {
            var PreviousClipboardText = Clipboard.GetText();
            var Txt = GetText();
            Clipboard.SetText(string.IsNullOrEmpty(Txt) ? " " : Txt);
            PushUndoableCommand(new Command
            {
                T = Command.Type.CopyToClipboard,
                ExecuteArgs = new List<object> { Clipboard.GetText() },
                UndoArgs = new List<object> { PreviousClipboardText }
            });
        }

        void Clear()
        {
            CursorPosition.X = 2;
            CharsToDraw.Clear();
            Refresh();
        }

        Tuple<int, int> DeletePreviousCharacter()
        {
            var DestX = CursorPosition.X - WolfFontHelper.ImageWidth;
            if (DestX < 0)
            {
                return new Tuple<int, int>(-1, -1);
            }

            var Index = DestX / WolfFontHelper.ImageWidth;
            var Char = CharsToDraw[Index];
            CharsToDraw.Remove(Char);
            CursorPosition.X = DestX;

            UpdateCharacterColors();
            InvalidateCharacters(Index);
            Update();

            return new Tuple<int, int>(Char.Codepoint, DestX);
        }

        void ExecuteCommand(Command command)
        {
            switch (command.T)
            {
                case Command.Type.AddNormalCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.ExecuteArgs[1];
                    AddNormalCharacter((int)command.ExecuteArgs[0]);
                    break;
                case Command.Type.AddColorCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.ExecuteArgs[2];
                    AddColorCharacter((Color)command.ExecuteArgs[0], (int)command.ExecuteArgs[1], true);
                    break;
                case Command.Type.AddSpecialCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.ExecuteArgs[1];
                    AddSpecialCharacter((int)command.ExecuteArgs[0], true);
                    break;
                case Command.Type.DeletePreviousCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.ExecuteArgs[0];
                    DeletePreviousCharacter();
                    break;
                case Command.Type.DeleteSelectedCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.ExecuteArgs[0];
                    DeleteSelectedCharacter();
                    break;
                case Command.Type.Clear:
                    Clear();
                    break;
                case Command.Type.CopyToClipboard:
                    Clipboard.SetText((string)command.ExecuteArgs[0]);
                    break;
                case Command.Type.PasteFromClipboard:
                    SetText((string)command.ExecuteArgs[0]);
                    break;
                case Command.Type.ToggleDrawColorCodes:
                    DrawColorCodes = (bool)command.ExecuteArgs[0];
                    Enabled = DrawColorCodes;
                    Focus();
                    CursorPosition.X = (int)command.ExecuteArgs[2];
                    Refresh();
                    break;
            }
        }

        void UndoCommand(Command command)
        {
            switch (command.T)
            {
                case Command.Type.AddNormalCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.UndoArgs[0];
                    DeletePreviousCharacter();
                    break;
                case Command.Type.AddColorCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.UndoArgs[0];
                    DeletePreviousCharacter();
                    DeletePreviousCharacter();
                    break;
                case Command.Type.AddSpecialCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.UndoArgs[0];
                    DeletePreviousCharacter();
                    break;
                case Command.Type.DeletePreviousCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.UndoArgs[1];
                    AddNormalCharacter((int)command.UndoArgs[0]);
                    break;
                case Command.Type.DeleteSelectedCharacter:
                    InvalidateCursor();
                    CursorPosition.X = (int)command.UndoArgs[1];
                    var DesiredCursorPosition = CursorPosition;
                    AddNormalCharacter((int)command.UndoArgs[0]);
                    CursorPosition = DesiredCursorPosition;
                    InvalidateCharacters(GetCharacterIndexFromCursorPosition());
                    break;
                case Command.Type.Clear:
                    SetText((string)command.UndoArgs[0]);
                    break;
                case Command.Type.CopyToClipboard:
                    Clipboard.SetText((string)command.UndoArgs[0]);
                    break;
                case Command.Type.PasteFromClipboard:
                    SetText((string)command.UndoArgs[0]);
                    break;
                case Command.Type.ToggleDrawColorCodes:
                    DrawColorCodes = (bool)command.UndoArgs[0];
                    Enabled = DrawColorCodes;
                    Focus();
                    CharsToDraw.Clear();
                    CharsToDraw.AddRange((List<Character>)command.UndoArgs[1]);
                    CursorPosition.X = (int)command.UndoArgs[2];
                    Refresh();
                    break;
            }
        }

        void PushUndoableCommand(Command command)
        {
            UndoneCommands.Clear();
            ExecutedCommands.Push(command);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left ||
                keyData == Keys.Right)
            {
                var Offset = keyData == Keys.Left ? -WolfFontHelper.ImageWidth : WolfFontHelper.ImageWidth;

                var DestX = CursorPosition.X + Offset;
                if (DestX / WolfFontHelper.ImageWidth <= CharsToDraw.Count &&
                    DestX > 0)
                {
                    InvalidateCursor();
                    CursorPosition.X = DestX;
                    InvalidateCursor();
                }

                return true;
            }
            else if (keyData == Keys.Delete)
            {
                int DeletedCodepoint = DeleteSelectedCharacter();
                if (DeletedCodepoint != -1)
                {
                    PushUndoableCommand(new Command
                    {
                        T = Command.Type.DeleteSelectedCharacter,
                        ExecuteArgs = new List<object> { CursorPosition.X },
                        UndoArgs = new List<object> { DeletedCodepoint, CursorPosition.X }
                    });
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        int DeleteSelectedCharacter()
        {
            var Index = GetCharacterIndexFromCursorPosition();
            if (CharsToDraw.Count == 0 ||
                Index >= CharsToDraw.Count)
            {
                return -1;
            }

            var Char = CharsToDraw[Index];
            CharsToDraw.Remove(Char);

            UpdateCharacterColors();
            InvalidateCharacters(Index);
            Update();

            return Char.Codepoint;
        }

        void AddNormalCharacter(int codepoint)
        {
            AddCharacter(codepoint, WolfColorUtil.DefaultRealColor);
        }

        void AddCharacter(int codepoint, Color color)
        {
            if (codepoint > WolfFontHelper.MaxCodepoint)
            {
                return;
            }

            AddCharacter(new Character
            {
                Img = WolfFont[codepoint],
                Codepoint = codepoint,
                Color = color
            });
        }

        void AddCharacter(Character character)
        {
            if (CharsToDraw.Count == MaxLength)
            {
                SystemSounds.Beep.Play();
                return;
            }

            var Index = CursorPosition.X / WolfFontHelper.ImageWidth;

            CharsToDraw.Insert(Index, character);

            UpdateCharacterColors();
            InvalidateCharacters(Index);
            CursorPosition.X += WolfFontHelper.ImageWidth;
            Update();
        }

        public void AddColorCharacter(Color color, int numberCodepoint, bool fromUndoRedoSystem = false)
        {
            var CursorXPositionBeforeAdd = CursorPosition.X;
            AddCharacter(WolfColorUtil.EscapeCharacter, color);
            AddCharacter(numberCodepoint, color);
            if (!fromUndoRedoSystem)
            {
                PushUndoableCommand(new Command
                {
                    T = Command.Type.AddColorCharacter,
                    ExecuteArgs = new List<object> { color, numberCodepoint, CursorXPositionBeforeAdd },
                    UndoArgs = new List<object> { CursorPosition.X }
                });
            }
        }

        public void AddSpecialCharacter(int codepoint, bool fromUndoRedoSystem = false)
        {
            var CursorXPositionBeforeAdd = CursorPosition.X;
            AddNormalCharacter(codepoint);
            if (!fromUndoRedoSystem)
            {
                PushUndoableCommand(new Command
                {
                    T = Command.Type.AddSpecialCharacter,
                    ExecuteArgs = new List<object> { codepoint, CursorXPositionBeforeAdd },
                    UndoArgs = new List<object> { CursorPosition.X }
                });
            }
        }

        void UpdateCharacterColors()
        {
            Color CurrentColor = WolfColorUtil.DefaultRealColor;

            for (int i = 0; i < CharsToDraw.Count; ++i)
            {
                var Character = CharsToDraw[i];
                if (Character.IsColorEscape)
                {
                    var NextIndex = i + 1;
                    if (NextIndex < CharsToDraw.Count)
                    {
                        var NextCharacter = CharsToDraw[NextIndex];
                        CurrentColor = WolfColorUtil.CodepointToRealColor(NextCharacter.Codepoint);
                    }
                    else
                    {
                        CurrentColor = WolfColorUtil.DefaultRealColor;
                    }
                }
                Character.Color = CurrentColor;
            }
        }

        void InvalidateCharacters(int characterIndex)
        {
            var PreviousIndex = characterIndex - 1;
            if (PreviousIndex >= 0 && CharsToDraw[PreviousIndex].IsColorEscape)
            {
                var InvalidatePosition = new Point(CursorPosition.X, CursorPosition.Y);
                InvalidatePosition.X -= WolfFontHelper.ImageWidth;
                Invalidate(new Rectangle(InvalidatePosition, new Size(MaxLength * WolfFontHelper.ImageWidth, WolfFontHelper.ImageHeight)));
            }
            else
            {
                Invalidate(new Rectangle(CursorPosition, new Size(MaxLength * WolfFontHelper.ImageWidth, WolfFontHelper.ImageHeight)));
            }
        }

        void InvalidateCursor()
        {
            Invalidate(new Rectangle(CursorPosition, new Size(WolfFontHelper.ImageWidth, 2)));
            Invalidate(new Rectangle(CursorPosition, new Size(2, WolfFontHelper.ImageHeight)));
            Invalidate(new Rectangle(new Point(CursorPosition.X + CursorSize.Width, CursorPosition.Y), new Size(2, WolfFontHelper.ImageHeight)));
            Invalidate(new Rectangle(new Point(CursorPosition.X, CursorPosition.Y + CursorSize.Height), new Size(WolfFontHelper.ImageWidth, 2)));
        }

        int GetCharacterIndexFromCursorPosition()
        {
            return CursorPosition.X / WolfFontHelper.ImageWidth;
        }

        Point GetCursorPositionFromCharacterIndex(int characterIndex)
        {
            return new Point(StartingCursorPosition.X + characterIndex * WolfFontHelper.ImageWidth, StartingCursorPosition.Y);
        }
    }
}
