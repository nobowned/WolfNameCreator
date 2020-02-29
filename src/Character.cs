using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace WolfNameCreator
{
    public class Character
    {
        readonly static Dictionary<Color, ImageAttributes> ColorImageAttributes = new Dictionary<Color, ImageAttributes>();

        public Character()
        {
            _color = WolfColorUtil.DefaultRealColor;
        }

        public Image Img;

        public int Codepoint;

        public bool IsColorEscape => Codepoint == WolfColorUtil.EscapeCharacter;

        Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;

                if (!ColorImageAttributes.ContainsKey(_color) && _color != Color.Empty)
                {
                    InitializeImageAttributesForColor(_color);
                }
            }
        }

        public ImageAttributes ImageAttributes => ColorImageAttributes[Color];

        void InitializeImageAttributesForColor(Color color)
        {
            var ColorMappings = new List<ColorMap>();
            for (int Index = 0; Index < 256; ++Index)
            {
                var ColorMap = new ColorMap { OldColor = Color.FromArgb(Index, Color.Black), NewColor = Color.FromArgb(Index, color) };
                ColorMappings.Add(ColorMap);
            }
            var ImageAttributes = new ImageAttributes();
            ImageAttributes.SetRemapTable(ColorMappings.ToArray());
            ColorImageAttributes[_color] = ImageAttributes;
        }
    }
}
