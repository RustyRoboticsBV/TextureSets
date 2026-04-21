#if UNITY_5_3_OR_NEWER
using System;
using UnityEngine;

namespace Rusty.Textures
{
    /// <summary>
    /// A Texture2D loader.
    /// </summary>
    public sealed class TextureLoader : TextureLoader<Texture2D>
    {
        /// <summary>
        /// Load a texture from a byte array representing a BMP.
        /// </summary>
        public override Texture2D LoadBmp(byte[] bytes) => Load(bytes);

        /// <summary>
        /// Load a texture from a byte array representing a PNG.
        /// </summary>
        public override Texture2D LoadPng(byte[] bytes) => Load(bytes);

        /// <summary>
        /// Load a texture from a byte array representing a JPG.
        /// </summary>
        public override Texture2D LoadJpg(byte[] bytes) => Load(bytes);

        /* Protected methods. */
        protected override void SetName(Texture2D texture, string name)
        {
            texture.name = name;
        }

        /* Private methods. */
        private Texture2D Load(byte[] bytes)
        {
            Texture2D texture = new Texture2D(1, 1);
            if (!texture.LoadImage(bytes))
                throw new Exception("Failed to load image.");
            return texture;
        }
    }
}
#endif