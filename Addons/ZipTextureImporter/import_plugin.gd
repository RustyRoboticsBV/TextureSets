@tool
extends EditorImportPlugin;

func _get_importer_name() -> String:
	return "zip_texture_set_importer";

func _get_visible_name() -> String:
	return "ZIP Texture";

func _get_recognized_extensions() -> PackedStringArray:
	return ["zip"];

func _get_save_extension() -> String:
	return "res";

func _get_resource_type() -> String:
	return "Texture2D";

func _get_preset_count() -> int:
	return 1;

func _get_preset_name(_preset_index: int) -> String:
	return "Default";

func _get_import_options(_path: String, _preset_index: int) -> Array:
	return [
		{
			"name": "r_channel_source",
			"default_value": ""
		},
		{
			"name": "g_channel_source",
			"default_value": ""
		},
		{
			"name": "b_channel_source",
			"default_value": ""
		},
		{
			"name": "a_channel_source",
			"default_value": ""
		},
		{
			"name": "use_mipmaps",
			"default_value": true
		},
		{
			"name": "is_normal_map",
			"default_value": false
		}
	];

func _get_option_visibility(_path: String, _option_name: StringName, _options: Dictionary) -> bool:
	return true;

func _import(source_file: String, save_path: String, options: Dictionary, _platform_variants: Array, _gen_files: Array) -> Error:
	var global_path : String = ProjectSettings.globalize_path(source_file);
	var texture : ImageTexture = ZipTextureLoader.Load(global_path,
		options.r_channel_source,
		options.g_channel_source,
		options.b_channel_source,
		options.a_channel_source
	);
	
	if options.use_mipmaps:
		texture.get_image().generate_mipmaps(true);
	
	if options.is_normal_map:
		texture.get_image().compress(Image.CompressMode.COMPRESS_S3TC, Image.COMPRESS_SOURCE_NORMAL);
	
	var save_file = "%s.%s" % [save_path, _get_save_extension()];
	return ResourceSaver.save(texture, save_file);
