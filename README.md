# ZIP Texture Loader
An Godot import plugin for loading ZIP archives as textures.

It can to combine several images from a ZIP archive into one texture resource. This allows you to, for example, maintain your metal, roughness and occlusion maps as separate files, but still load them as one texture.

It can handle `.bmp`, `.png`, `.jpg`, `.svg`, `.webp`, `.tga`, `.dds` and `.ktx` files.

## Installation
1. Download and drop the files `Addons` folder in your Godot project root directory.
2. Enable the import plugin under `Project` => `Project Settings...` => `Plugins` => `ZIP Texture Importer`.

## Usage
Any ZIP file in your resource folder can be imported as a `ZIP Texture`. In the importer, you can specify which images the red, green, blue and alpha channels should be taken from. Other miscellaneous settings are also available, such as selecting the compression type, enabling mipmaps, inverting channels and marking the texture as a normal map.

Several presets are available in the importer menu for common use-cases, such as importing occlusion/roughness/metal or albedo/alpha textures.
