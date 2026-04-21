using Godot;

namespace Rusty.Textures
{
    /// <summary>
    /// A base class for channel packers.
    /// </summary>
    public abstract class ChannelPacker<TextureT, ColorT>
    {
        /* Public methods. */
        /// <summary>
        /// Pack four texture channels into one.
        /// </summary>
        public TextureT Pack(TextureT r, TextureT g, TextureT b, TextureT a)
        {
            if (r == null && g == null && b == null && a == null)
                return default;

            // Get width and height.
            int width = Mathf.Min(Mathf.Min(GetWidth(r), GetWidth(g)), Mathf.Min(GetWidth(b), GetWidth(a)));
            int height = Mathf.Min(Mathf.Min(GetHeight(r), GetHeight(g)), Mathf.Min(GetHeight(b), GetHeight(a)));

            // Create combined texture.
            TextureT texture = Create(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get combined pixel.
                    ColorT pixelR = GetPixel(r, x, y);
                    ColorT pixelG = GetPixel(g, x, y);
                    ColorT pixelB = GetPixel(b, x, y);
                    ColorT pixelA = GetPixel(a, x, y);
                    ColorT pixel = Combine(pixelR, pixelG, pixelB, pixelA);

                    // Write to texture.
                    SetPixel(texture, x, y, pixel);
                }
            }

            // Finish texture and return.
            FinishTexture(texture);
            return texture;
        }

        /* Protected methods. */
        /// <summary>
        /// Create a texture.
        /// </summary>
        protected abstract TextureT Create(int width, int height);

        /// <summary>
        /// Get the minimum width of four textures.
        /// </summary>
        protected abstract int GetWidth(TextureT texture);

        /// <summary>
        /// Get the minimum height of four textures.
        /// </summary>
        protected abstract int GetHeight(TextureT texture);

        /// <summary>
        /// Get a pixel from a texture.
        /// </summary>
        protected abstract ColorT GetPixel(TextureT texture, int x, int y);

        /// <summary>
        /// Set a pixel of a texture.
        /// </summary>
        protected abstract void SetPixel(TextureT texture, int x, int y, ColorT color);

        /// <summary>
        /// Combine four color channels into one.
        /// </summary>
        protected abstract ColorT Combine(ColorT r, ColorT g, ColorT b, ColorT a);

        /// <summary>
        /// Get the red value of a color.
        /// </summary>
        protected abstract byte GetR(ColorT color);

        /// <summary>
        /// Get the green value of a color.
        /// </summary>
        protected abstract byte GetG(ColorT color);

        /// <summary>
        /// Get the blue value of a color.
        /// </summary>
        protected abstract byte GetB(ColorT color);

        /// <summary>
        /// Get the alpha value of a color.
        /// </summary>
        protected abstract byte GetA(ColorT color);

        /// <summary>
        /// Finish the texture.
        /// </summary>
        protected virtual void FinishTexture(TextureT texture) { }
    }
}