# Texture Sets
A C# utility for loading texture sets from ZIP archive. It can be used in the Unity and Godot game engines.

In both engines, the ZIP archives will be loaded as `TextureSet` objects (which inherit `ScriptableObject` in Unity, and a `Resource` in Godot). These contain a set of name-texture pairs. It also exposes several methods for adding, removing and getting textures from the set.

ZIP files may contain BMP, JPG and PNG images. Files with `normal` in the title will be treated as normal maps.

## INI Loading

You can control how textures are loaded by including an INI file in the ZIP archive. This allows you to:
- Define a texture name (using the section name).
- Mark a texture as a normal map, using the `normal_map` property.
- Define a texture's sources. A source can either be an image path, or a value between `0.0` and `1.0`. You can either define one source using the `source` property, or target specific channels by using things like `source_r` or `source_rgb`. The latter allows for automatic channel packing of several textures into one.

The example below loads an albedo-alpha, normal, metal-roughness-occlusion and emission map.

```
[AlbedoAlpha]
source_rgb = "Albedo.png"
source_a = 1.0

[Normal]
source = "Normal.png"
normal_map = true

[MetalRoughness]
source_r = "Metal.png"
source_g = "Roughness.png"
source_b = "Occlusion.png"

[Emission]
source = "Emission.png"
```
