# ZIP Texture Loader
An Godot import plugin for loading ZIP archives as textures.

It can to combine several images from a ZIP archive into one texture resource. This allows you to, for example, maintain your metal, roughness and occlusion maps as separate fles, but still load them as one texture.

## Installation
1. Make sure that you are using a C# build of Godot.
2. Download and drop the files `Addons` folder in your Godot project root directory.
3. Press the `Build Project`.
4. Enable the import plugin under `Project` => `Project Settings...` => `Plugins` => `ZIP Texture Importer`.

## Usage
Any ZIP file in your resource folder can be imported as a `ZIP Texture`. In the importer, you can specify which images the red, green, blue and alpha channels should be taken from. Other miscellaneous settings are also available, such as enabling mipmaps and normal map mode.

Several presets are available in the importer menu for common use-cases, such as importing occlusion/roughness/metal or albedo/alpha textures.