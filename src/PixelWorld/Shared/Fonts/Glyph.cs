namespace PixelWorld.Shared.Fonts
{
    public class Glyph
    {
        public int Height { get; }
        public int Width { get; }
        public bool[,] Data { get; }

        public Glyph(int width, int height, bool[,] data)
        {
            Width = width;
            Height = height;
            Data = data;
        }
    }
}