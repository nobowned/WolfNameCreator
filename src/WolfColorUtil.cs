using System.Drawing;

namespace WolfNameCreator
{
    public static class WolfColorUtil
    {
        public const int WolfBlack = 0;
        public const int WolfRed = 1;
        public const int WolfGreen = 2;
        public const int WolfYellow = 3;
        public const int WolfBlue = 4;
        public const int WolfCyan = 5;
        public const int WolfPurple = 6;
        public const int WolfWhite = 7;
        public const int EscapeCharacter = '^';

        public static int DefaultWolfColor = WolfWhite;
        public static Color DefaultRealColor = WolfColorToRealColor(DefaultWolfColor);

        public static Color WolfColorToRealColor(int wolfColor)
        {
            switch (wolfColor)
            {
                case WolfBlack:
                    return Color.Black;
                case WolfRed:
                    return Color.Red;
                case WolfGreen:
                    return Color.Green;
                case WolfYellow:
                    return Color.Yellow;
                case WolfBlue:
                    return Color.Blue;
                case WolfCyan:
                    return Color.Cyan;
                case WolfPurple:
                    return Color.Purple;
                default:
                    return Color.White;
            }
        }

        public static int RealColorToWolfColor(Color color)
        {
            if (color == Color.Black)
            {
                return WolfBlack;
            }
            if (color == Color.Red)
            {
                return WolfRed;
            }
            if (color == Color.Green)
            {
                return WolfGreen;
            }
            if (color == Color.Yellow)
            {
                return WolfYellow;
            }
            if (color == Color.Blue)
            {
                return WolfBlue;
            }
            if (color == Color.Cyan)
            {
                return WolfCyan;
            }
            if (color == Color.Purple)
            {
                return WolfPurple;
            }
            return WolfWhite;
        }

        public static int RealColorToCodepoint(Color color)
        {
            return RealColorToWolfColor(color).ToString()[0];
        }

        public static int CodepointToWolfColor(int codepoint)
        {
            return codepoint % 8;
        }

        public static Color CodepointToRealColor(int codepoint)
        {
            return WolfColorToRealColor(CodepointToWolfColor(codepoint));
        }
    }
}
