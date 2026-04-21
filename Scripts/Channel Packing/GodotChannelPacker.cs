#if GODOT
using Godot;

namespace Rusty.Textures
{
    /// <summary>
    /// An Image channel packer.
    /// </summary>
    public sealed class ChannelPacker : ChannelPacker<Image, Color>
    {
        /* Protected methods. */
        protected override Image Create(int width, int height)
        {
            if (width == 0 || height == 0)
                return null;

            Image image = new();
            image.SetData(width, height, false, Image.Format.Rgba8, new byte[width * height * 4]);
            return image;
        }

        protected override int GetWidth(Image image)
            => image?.GetWidth() ?? int.MaxValue;

        protected override int GetHeight(Image image)
            => image?.GetHeight() ?? int.MaxValue;

        protected override Color GetPixel(Image image, int x, int y)
            => image?.GetPixel(x, y) ?? new Color(0, 0, 0, 1);

        protected override void SetPixel(Image image, int x, int y, Color color)
            => image?.SetPixel(x, y, color);

        protected override Color Combine(Color r, Color g, Color b, Color a)
            => new Color(GetR(r) / 255f, GetG(g) / 255f, GetB(b) / 255f, GetA(a) / 255f);

        protected override byte GetR(Color color)
            => (byte)color.R8;

        protected override byte GetG(Color color)
            => (byte)color.G8;

        protected override byte GetB(Color color)
            => (byte)color.B8;

        protected override byte GetA(Color color)
            => (byte)color.A8;
    }
}
#endif