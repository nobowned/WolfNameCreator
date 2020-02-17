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

        public static Color DefaultColor = Color.White;

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
