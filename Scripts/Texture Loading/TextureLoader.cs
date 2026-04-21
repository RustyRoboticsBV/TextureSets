using System;
using System.IO;
using System.IO.Compression;

namespace Rusty.Textures
{
    /// <summary>
    /// A base class for texture loaders.
    /// </summary>
    public abstract class TextureLoader<TextureT>
    {
        /// <summary>
        /// Load a texture from a file at some global path.
        /// </summary>
        public TextureT Load(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            return LoadAndName(bytes, filePath);
        }

        /// <summary>
        /// Load a texture from a zip archive entry.
        /// </summary>
        public TextureT Load(ZipArchiveEntry archiveEntry)
        {
            using (Stream stream = archiveEntry.Open())
            using (MemoryStream reader = new MemoryStream())
            {
                stream.CopyTo(reader);
                byte[] bytes = reader.ToArray();
                return LoadAndName(bytes, archiveEntry.Name);
            }
        }
        
        /// <summary>
        /// Load a texture from a byte array representing a BMP.
        /// </summary>
        public abstract TextureT LoadBmp(byte[] bytes);

        /// <summary>
        /// Load a texture from a byte array representing a PNG.
        /// </summary>
        public abstract TextureT LoadPng(byte[] bytes);

        /// <summary>
        /// Load a texture from a byte array representing a JPG.
        /// </summary>
        public abstract TextureT LoadJpg(byte[] bytes);
        
        /* Protected methods. */
        /// <summary>
        /// Set the name of a texture.
        /// </summary>
        protected abstract void SetName(TextureT texture, string name);

        /* Private methods. */
        private TextureT LoadAndName(byte[] bytes, string fileAndExtension)
        {
            TextureT texture = Load(bytes, Path.GetExtension(fileAndExtension));
            SetName(texture, Path.GetFileNameWithoutExtension(fileAndExtension));
            return texture;
        }

        private TextureT Load(byte[] bytes, string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".bmp":
                    return LoadBmp(bytes);
                case ".png":
                    return LoadPng(bytes);
                case ".jpg":
                case ".jpeg":
                    return LoadJpg(bytes);
                default:
                    throw new ArgumentException($"Invalid extension '{extension}'.", nameof(extension));
            }
        }
    }
}