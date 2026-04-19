# Texture Sets
A C# utility for loading texture sets from ZIP archive. It can be used in the Unity and Godot game engines.

In both engines, the ZIP archives will be loaded as `TextureSet` objects (which inherit `ScriptableObject` in Unity, and a `Resource` in Godot). These contain a set of name-texture pairs. It also exposes several methods for adding, removing and getting textures from the set.

ZIP files may contain BMP, JPG and PNG images. Files with `normal` in the title will be treated as normal maps.

## INI Loading

You can control how textures are loaded by including an INI file in the ZIP archive. This allows you to mark images as normal maps, or pack several images into one. When combining images, omitted channels are always treated as black.

The example below loads an albedo-alpha, normal, metal-roughness-occlusion and emission map.

```
[AlbedoAlpha]
source_rgb = "Albedo.png"
source_a = "Alpha.png"

[Normal]
source = "Normal.png"
normal_map = 1

[MetalRoughness]
source_r = "Metal.png"
source_g = "Roughness.png"
source_b = "Occlusion.png"

[Emission]
source = "Emission.png"
```
