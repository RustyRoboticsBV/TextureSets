#if GODOT
using System;
using Godot;

namespace Rusty.Textures
{
    /// <summary>
    /// An ImageTexture loader.
    /// </summary>
    public sealed class TextureLoader : TextureLoader<ImageTexture>
    {
        /* Public methods. */
        public override ImageTexture LoadBmp(byte[] bytes)
        {
            Image image = new Image();
            Error err = image.LoadBmpFromBuffer(bytes);

            if (err != Error.Ok)
                throw new System.Exception($"Failed to load BMP: {err}");

            return ImageTexture.CreateFromImage(image);
        }

        public override ImageTexture LoadPng(byte[] bytes)
        {
            Image image = new Image();
            Error err = image.LoadPngFromBuffer(bytes);

            if (err != Error.Ok)
                throw new System.Exception($"Failed to load PNG: {err}");

            return ImageTexture.CreateFromImage(image);
        }

        public override ImageTexture LoadJpg(byte[] bytes)
        {
            Image image = new Image();
            Error err = image.LoadJpgFromBuffer(bytes);

            if (err != Error.Ok)
                throw new System.Exception($"Failed to load JPG: {err}");

            return ImageTexture.CreateFromImage(image);
        }

        /* Protected methods. */
        protected override void SetName(ImageTexture texture, string name)
        {
            texture.ResourceName = name;
        }
    }
}
#endif