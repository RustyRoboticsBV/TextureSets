using System;

namespace Rusty.Textures
{
    /// <summary>
    /// An image made of pixels.
    /// </summary>
    public class Image
    {
        /* Fields. */
        public Pixel[] pixels;
        public readonly int width;
        public readonly int height;
        public readonly int size;

        /* Indexers. */
        public Pixel this[int index] => pixels[index];
        public Pixel this[int x, int y] => this[x + y * width];

        /* Public properties. */
        public static Image Empty => new Image(new Pixel[0], 0, 0) { };

        /* Constructors. */
        public Image(Pixel[] pixels, int width, int height)
        {
            if (pixels.Length != width * height)
            {
                throw new ArgumentException($"The length of {nameof(pixels)} does not match {nameof(width)} * {nameof(height)} "
                    + $"({pixels.Length} and {width} * {height} = {width * height}).");
            }

            this.pixels = pixels;
            this.width = width;
            this.height = height;
            size = width * height;
        }

        /* Public methods. */
        public Pixel TryGetPixel(int x, int y)
        {
            int index = x + y * width;
            if (index < size)
                return this[index];
            else
                return Pixel.Clear;
        }
    }
}