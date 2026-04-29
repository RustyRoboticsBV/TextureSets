# ZIP Texture Loader
<p align="center">
  <img src="Images/Example.png" width="800">
</p>

An Godot import plugin for loading ZIP archives as textures.

It can to combine several images from a ZIP archive into one texture resource. This allows you to, for example, maintain your metal, roughness and occlusion maps as separate files, but still load them as one texture.

It can handle all image formats that Godot can handle by default: `.bmp`, `.png`, `.jpg`, `.svg`, `.webp`, `.tga`, `.dds` and `.ktx`.

## Installation
1. Download and drop the `Addons` directory into your Godot project directory.
2. Enable the import plugin under `Project` => `Project Settings...` => `Plugins` => `ZIP Texture Importer`.

## Usage
Any ZIP file in your resource folder can be imported as a `ZIP Texture`. In the importer, you can:
- Specify which images the red, green, blue and alpha channels should be taken from.
- Inverting specific channels.
- Select the compression type.
- Enable mipmaps.
- Mark the texture as a normal map.

Several importer presets are available for importing common texture types, such as albedo, normal, metal, roughness, occlusion, ORM and emission.
