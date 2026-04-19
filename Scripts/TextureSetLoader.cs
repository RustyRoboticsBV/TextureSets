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

                        string lowercase = entry.Name.ToLower();
                        string extension = lowercase.Substring(lowercase.LastIndexOf('.') + 1);
                        string name = entry.Name.Substring(0, entry.Name.Length - extension.Length);
                        if (extension == "bmp")
                        {
                            names.Add(name);
                            textures.Add(CreateBmpTexture(name, bytes));
                        }
                        else if (extension == "png")
                        {
                            names.Add(name);
                            textures.Add(CreatePngTexture(name, bytes));
                        }
                        else if (extension == "jpg" || extension == "jpeg")
                        {
                            names.Add(name);
                            textures.Add(CreateJpgTexture(name, bytes));
                        }
                        else if (extension == "ini")
                        {

                        }
                    }
                }
            }

            // Create texture set.
            TextureSet set = TextureSet.CreateNew();
            for (int i = 0; i < names.Count; i++)
            {
                set.Add(names[i], textures[i]);
            }
            return set;
        }

        /* Private methods. */
        private static Texture2D CreateBmpTexture(string name, byte[] bytes)
        {
#if GODOT
            Image image = new Image();
            image.LoadBmpFromBuffer(bytes);
            return MakeTexture(image);
#elif UNITY_5_3_OR_NEWER
            return LoadUnityTexture(name, bytes);
#else
            throw new NotImplementedException($"{nameof(TextureSetLoader)}.{nameof(CreatePngTexture)} cannot be used in this context.");
#endif
        }

        private static Texture2D CreatePngTexture(string name, byte[] bytes)
        {
#if GODOT
            Image image = new Image();
            image.LoadPngFromBuffer(bytes);
            return MakeTexture(image);
#elif UNITY_5_3_OR_NEWER
            return LoadUnityTexture(name, bytes);
#else
            throw new NotImplementedException($"{nameof(TextureSetLoader)}.{nameof(CreatePngTexture)} cannot be used in this context.");
#endif
        }

        private static Texture2D CreateJpgTexture(string name, byte[] bytes)
        {
#if GODOT
            Image image = new Image();
            image.LoadJpgFromBuffer(bytes);
            return MakeTexture(image);
#elif UNITY_5_3_OR_NEWER
            return LoadUnityTexture(name, bytes);
#else
            throw new NotImplementedException($"{nameof(TextureSetLoader)}.{nameof(CreatePngTexture)} cannot be used in this context.");
#endif
        }

#if UNITY_5_3_OR_NEWER
        /// <summary>
        /// Load a PNG, JPG or EXR texture as a Unity texture.
        /// </summary>
        private static Texture2D LoadUnityTexture(string name, byte[] bytes)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            texture.name = name;
            return texture;
        }
#endif

#if GODOT
        /// <summary>
        /// Create a Godot texture from a Godot image.
        /// </summary>
        private static Texture2D MakeTexture(Image image)
        {
            ImageTexture texture = new ImageTexture();
            texture.SetImage(image);
            texture.ResourceName = name;
            return texture;
        }
#endif
    }
}