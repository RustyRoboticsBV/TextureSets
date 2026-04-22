using Godot;
using System;
using System.IO;
using System.IO.Compression;

namespace Rusty.Textures;

[GlobalClass]
public partial class ZipTextureLoader : Node
{
    /* Public methods. */
    /// <summary>
    /// Load a ZIP archive at some path as a texture.
    /// </summary>
    public static ImageTexture Load(string path, string r, string g, string b, string a)
    {
        TextureLoader loader = new TextureLoader();
        ChannelPacker packer = new ChannelPacker();

        try
        {
            // Load textures from the ZIP.
            ImageTexture imgR = null;
            ImageTexture imgB = null;
            ImageTexture imgG = null;
            ImageTexture imgA = null;
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string filename = Path.GetFileNameWithoutExtension(entry.Name);
                    if (r == filename)
                        imgR = loader.Load(entry);
                    if (g == filename)
                        imgG = loader.Load(entry);
                    if (b == filename)
                        imgB = loader.Load(entry);
                    if (a == filename)
                        imgA = loader.Load(entry);
                }
            }

            // Ensure that all channels were found.
            if (!string.IsNullOrEmpty(r) && imgR == null)
                throw new ArgumentException($"The zipped texture '{r}' could not be found!");
            if (!string.IsNullOrEmpty(g) && imgG == null)
                throw new ArgumentException($"The zipped texture '{g}' could not be found!");
            if (!string.IsNullOrEmpty(b) && imgB == null)
                throw new ArgumentException($"The zipped texture '{b}' could not be found!");
            if (!string.IsNullOrEmpty(a) && imgA == null)
                throw new ArgumentException($"The zipped texture '{a}' could not be found!");

            // Combine.
            if (imgR == null && imgG == null && imgB == null && imgA == null)
                return new ImageTexture();

            Image image = packer.Pack(
                imgR?.GetImage(),
                imgG?.GetImage(),
                imgB?.GetImage(),
                imgA?.GetImage()
            );

            // Create texture.
            ImageTexture result = new ImageTexture();
            result.SetImage(image);
            return result;
        }
        catch (Exception ex)
        {
            GD.PushError(ex.Message);
            return null;
        }
    }
}