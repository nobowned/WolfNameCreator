using System.Drawing;

namespace WolfNameCreator
{
    public static class WolfNameHelper
    {
        public const int ImageWidth = 16;
        public const int ImageHeight = 16;
        public const int MaxCodepoint = 125;

        public static bool IsInvalidCodepoint(int codepoint)
        {
            return codepoint == 0 || codepoint == 4 || codepoint == 5 ||
                    codepoint == 7 || codepoint == 10 || codepoint == 11 ||
                    codepoint == 12 || codepoint == 13 || codepoint == 14 ||
                    codepoint == 15 || codepoint == 22;
        }

        public static Point GetCodepointPosition(int codepoint)
        {
            return new Point((codepoint % ImageWidth) * ImageWidth, (codepoint / ImageHeight) * ImageHeight);
        }

        public static Size GetCodepointSize()
        {
            return new Size(ImageWidth, ImageHeight);
        }

        public static Rectangle GetCodepointBounds(int codepoint)
        {
            return new Rectangle(GetCodepointPosition(codepoint), GetCodepointSize());
        }

        public static void RemoveInvalidCodepoints(string removeFromImagePath, string destImagePath)
        {
            // NOTE: Must be in ascending order!
            var RemovalCodepoints = new int[]
            {
                0, 4, 5, 10, 12, 13, 14, 15, 22
            };

            using (var Img = new Bitmap(removeFromImagePath))
            {
                int CodepointsRemoved = 0;
                foreach (var RemovalCodepoint in RemovalCodepoints)
                {
                    for (int Codepoint = RemovalCodepoint - CodepointsRemoved; Codepoint < 256 - CodepointsRemoved; ++Codepoint)
                    {
                        var DstStart = GetCodepointPosition(Codepoint);
                        var SrcPoint = GetCodepointPosition(Codepoint + 1);

                        int SrcX = SrcPoint.X;
                        for (int DestX = DstStart.X; DestX < DstStart.X + ImageWidth; ++DestX, ++SrcX)
                        {
                            int SrcY = SrcPoint.Y;
                            for (int DestY = DstStart.Y; DestY < DstStart.Y + ImageHeight; ++DestY, ++SrcY)
                            {
                                var SrcPixelColor = Codepoint == 255 ? Color.Transparent : Img.GetPixel(SrcX, SrcY);
                                Img.SetPixel(DestX, DestY, SrcPixelColor);
                            }
                        }
                    }
                    CodepointsRemoved++;
                }

                using (var DestImage = new Bitmap(23 * ImageWidth, ImageHeight))
                {
                    for (int x = 0; x < 256; ++x)
                    {
                        for (int y = 0; y < ImageHeight; ++y)
                        {
                            DestImage.SetPixel(x, y, Img.GetPixel(x, y));
                        }
                    }

                    int DestX = 256;
                    for (int SrcX = 0; SrcX < 7 * ImageWidth; ++SrcX, ++DestX)
                    {
                        int DestY = 0;
                        for (int SrcY = ImageHeight; SrcY < 32; ++SrcY, ++DestY)
                        {
                            DestImage.SetPixel(DestX, DestY, Img.GetPixel(SrcX, SrcY));
                        }
                    }

                    DestImage.Save(destImagePath);
                }
            }
        }

        public static void WriteValidControlCodepointsToFile(string fileName)
        {
            using (var SomeFile = new System.IO.StreamWriter(fileName))
            {
                SomeFile.Write("set name \"");
                for (byte i = 0; i < 32; ++i)
                {
                    // Invalid/unnecessary codepoints
                    if (IsInvalidCodepoint(i))
                    {
                        continue;
                    }

                    SomeFile.Write((char)i);
                }
                SomeFile.Write("\"");
                SomeFile.Write((char)0);
            }
        }
    }
}
