using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#elif GODOT
using Godot;
#endif

namespace Rusty.Textures
{
#if GODOT
    [GlobalClass]
    public sealed partial class TextureSetLoader : Node
#else
    public static class TextureSetLoader
#endif
    {
        /* Public methods. */
        /// <summary>
        /// Load a texture set from a ZIP file at some path.
        /// </summary>
        public static TextureSet Load(string path)
        {
            // Load textures from the ZIP.
            List<string> names = new List<string>();
            List<Texture2D> textures = new List<Texture2D>();
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    using (Stream stream = entry.Open())
                    using (MemoryStream reader = new MemoryStream())
                    {
                        stream.CopyTo(reader);
                        byte[] bytes = reader.ToArray();

                        if (entry.Name.EndsWith(".png"))
                        {
                            string name = entry.Name.Substring(0, entry.Name.Length - 4);
                            names.Add(name);
                            textures.Add(CreatePngTexture(name, bytes));
                        }
                    }
                }
            }

            // Create texture set.
            TextureSet set = new TextureSet();
            for (int i = 0; i < names.Count; i++)
            {
                set.Add(names[i], textures[i]);
            }
            return set;
        }

        /* Private methods. */
        private static Texture2D CreatePngTexture(string name, byte[] bytes)
        {
#if GODOT
            Image image = new Image();
            image.LoadPngFromBuffer(bytes);

            ImageTexture texture = new ImageTexture();
            texture.SetImage(image);
            texture.ResourceName = name;
            return texture;
#elif UNITY_5_3_OR_NEWER
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            return texture;
#else
            throw new NotImplementedException($"{nameof(TextureSetLoader)}.{nameof(CreatePngTexture)} cannot be used in this context.");
#endif
        }
    }
}