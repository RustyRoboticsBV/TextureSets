using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

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
            List<bool> normalmap = new List<bool>();
            IniFile ini = new IniFile();
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
                        string name = entry.Name.Substring(0, entry.Name.Length - extension.Length - 1);
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
                            using (var stream2 = entry.Open())
                            using (var reader2 = new StreamReader(stream2, Encoding.UTF8))
                            {
                                ini.Load(reader2);
                            }
                        }

                        normalmap.Add(name.ToLower().Contains("normal"));
                    }
                }
            }

            // Apply INI.
            if (ini != null)
            {
                TextureSet set2 = new TextureSet();

                foreach (string sectionKey in ini.Keys)
                {
                    IniSection section = ini[sectionKey];
                    foreach (string valueKey in section.Keys)
                    {
                        IniValue value = section[valueKey];
                        string lowercase = valueKey.ToLower();
                        if (lowercase == "normal_map")
                            Debug.Log(sectionKey + " is a normal map!");
                    }
                }
            }

            // Create texture set.
            TextureSet set = TextureSet.CreateNew();
            for (int i = 0; i < names.Count; i++)
            {
                if (normalmap[i])
                    textures[i] = ConvertToNormalMap(textures[i]);
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
            throw Throw(nameof(CreateBmpTexture));
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
            throw Throw(nameof(CreateJpgTexture));
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
            throw Throw(nameof(CreatePngTexture));
#endif
        }

#if UNITY_5_3_OR_NEWER
        /// <summary>
        /// Load a BMP, JPG, PNG or EXR texture as a Unity texture.
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

        private static Texture2D ConvertToNormalMap(Texture2D source)
        {
#if UNITY_5_3_OR_NEWER
            Texture2D normalTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false, true); // linear
            normalTex.name = source.name;

            for (int y = 0; y < source.height; y++)
            {
                for (int x = 0; x < source.width; x++)
                {
                    Color c = source.GetPixel(x, y);

                    // Convert from [0,1] to [-1,1]
                    float nx = c.r * 2f - 1f;
                    float ny = c.g * 2f - 1f;

                    // Reconstruct Z
                    float nz = Mathf.Sqrt(1f - Mathf.Clamp01(nx * nx + ny * ny));

                    Vector3 normal = new Vector3(nx, ny, nz).normalized;

                    // Back to [0,1]
                    Color newColor = new Color(
                        normal.x * 0.5f + 0.5f,
                        normal.y * 0.5f + 0.5f,
                        normal.z * 0.5f + 0.5f
                    );

                    normalTex.SetPixel(x, y, newColor);
                }
            }

            normalTex.Apply();
            return normalTex;
#elif GODOT
            throw Throw(nameof(ConvertToNormalMap));
#else
            throw Throw(nameof(ConvertToNormalMap));
#endif
        }

        private static NotImplementedException Throw(string name)
        {
            return new NotImplementedException($"{nameof(TextureSetLoader)}.{name} cannot be used in this context.");
        }
    }
}