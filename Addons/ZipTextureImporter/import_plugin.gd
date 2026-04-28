@tool
extends EditorImportPlugin;

const Presets = [
	"Default",
	"Albedo",
	"Albedo + Alpha",
	"Normal",
	"Metal",
	"Roughness",
	"Occlusion",
	"ORM",
	"Emission"
];

const Red = [
	"",
	"Albedo",
	"Albedo",
	"Normal",
	"Metal",
	"",
	"",
	"Occlusion",
	"Emission"
];

const Green = [
	"",
	"Albedo",
	"Albedo",
	"Normal",
	"",
	"Roughness",
	"",
	"Roughness",
	"Emission"
];

const Blue = [
	"",
	"Albedo",
	"Albedo",
	"Normal",
	"",
	"",
	"Occlusion",
	"Metal",
	"Emission"
];

const Alpha = [
	"",
	"Albedo",
	"Alpha",
	"",
	"",
	"",
	"",
	"",
	"Emission"
];

const Compression = [
	"None",
	"BPTC",
	"BPTC",
	"None",
	"BPTC",
	"BPTC",
	"BPTC",
	"BPTC",
	"Emission"
];

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
	return Presets.size();

func _get_preset_name(preset_index: int) -> String:
	return Presets[preset_index];

func _get_import_options(_path: String, preset_index: int) -> Array:
	return [
		{
			"name": "r_channel_source",
			"default_value": Red[preset_index]
		},
		{
			"name": "g_channel_source",
			"default_value": Green[preset_index]
		},
		{
			"name": "b_channel_source",
			"default_value": Blue[preset_index]
		},
		{
			"name": "a_channel_source",
			"default_value": Alpha[preset_index]
		},
		{
			"name": "compression",
			"default_value": Compression[preset_index],
			"property_hint": PROPERTY_HINT_ENUM,
			"hint_string": "None,ASTC,BPTC,ETC,ETC2,S3TC"
		},
		{
			"name": "use_mipmaps",
			"default_value": true
		},
		{
			"name": "is_normal_map",
			"default_value": Presets[preset_index] == "Normal"
		}
	];

func _get_option_visibility(_path: String, option_name: StringName, options: Dictionary) -> bool:
	if option_name == "compression" && options.is_normal_map:
		return true;
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
		texture.get_image().compress(Image.CompressMode.COMPRESS_S3TC, Image.CompressSource.COMPRESS_SOURCE_NORMAL);
	else:
		match options.compression:
			"ASTC":
				texture.get_image().compress(Image.CompressMode.COMPRESS_ASTC, Image.CompressSource.COMPRESS_SOURCE_GENERIC);
			"BPTC":
				texture.get_image().compress(Image.CompressMode.COMPRESS_BPTC, Image.CompressSource.COMPRESS_SOURCE_GENERIC);
			"ETC":
				texture.get_image().compress(Image.CompressMode.COMPRESS_ETC, Image.CompressSource.COMPRESS_SOURCE_GENERIC);
			"ETC2":
				texture.get_image().compress(Image.CompressMode.COMPRESS_ETC2, Image.CompressSource.COMPRESS_SOURCE_GENERIC);
			"S3TC":
				texture.get_image().compress(Image.CompressMode.COMPRESS_S3TC, Image.CompressSource.COMPRESS_SOURCE_GENERIC);
	
	var save_file = "%s.%s" % [save_path, _get_save_extension()];
	return ResourceSaver.save(texture, save_file);
