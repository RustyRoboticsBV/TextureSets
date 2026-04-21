namespace Rusty.Textures
{
    /// <summary>
    /// A single pixel.
    /// </summary>
    public struct Pixel
    {
        /* Fields. */
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        /* Public properties. */
        public static Pixel Clear => new Pixel(0, 0, 0, 0);
        
        /* Constructors. */
        public Pixel(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}