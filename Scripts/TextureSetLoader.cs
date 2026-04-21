using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;


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
        /* Fields. */
        private static TextureLoader loader = new TextureLoader();

        /* Public methods. */
        /// <summary>
        /// Load a texture set from a ZIP file at some path.
        /// </summary>
        public static TextureSet Load(string path)
        {
            try
            {
                // Load textures from the ZIP.
                List<string> names = new List<string>();
                List<Texture2D> textures = new List<Texture2D>();
                List<bool> normalmap = new List<bool>();
                List<bool> mipmap = new List<bool>();
                IniFile ini = new IniFile();
                using (ZipArchive archive = ZipFile.OpenRead(path))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string lowercase = entry.Name.ToLower();
                        string extension = lowercase.Substring(lowercase.LastIndexOf('.') + 1);
                        string name = entry.Name.Substring(0, entry.Name.Length - extension.Length - 1);
                        switch (extension)
                        {
                            case "bmp":
                            case "png":
                            case "jpg":
                            case "jpeg":
                                names.Add(name);
                                textures.Add(loader.Load(entry));
                                normalmap.Add(lowercase.Contains("normal"));
                                break;
                            case "ini":
                                using (var stream = entry.Open())
                                using (var reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    ini.Load(reader);
                                }

                                normalmap.Add(name.ToLower().Contains("normal"));
                                break;
                        }
                    }
                }

                // Apply INI.
                if (ini != null)
                {
                    TextureSet set2 = TextureSet.CreateNew();

                    foreach (string sectionKey in ini.Keys)
                    {
                        IniSection section = ini[sectionKey];
                        foreach (string valueKey in section.Keys)
                        {
                            IniValue value = section[valueKey];
                            string lowercase = valueKey.ToLower();
                            // TODO: interpret
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
            catch (Exception exception)
            {
                ReportError(exception.Message);
                return null;
            }
        }

        /* Private methods. */
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
            return source;
#else
            throw Throw(nameof(ConvertToNormalMap));
#endif
        }

        private static NotImplementedException Throw(string name)
        {
            return new NotImplementedException($"{nameof(TextureSetLoader)}.{name} cannot be used in this context.");
        }

        private static void ReportError(string message)
        {
#if UNITY_5_3_OR_NEWER
            Debug.LogError(message);
#elif GODOT
            GD.PushError(message);
#endif
        }
    }
}