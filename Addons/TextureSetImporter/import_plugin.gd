@tool
extends EditorImportPlugin;

func _get_importer_name() -> String:
	return "texture_set_importer";

func _get_visible_name() -> String:
	return "Texture Set";

func _get_recognized_extensions() -> PackedStringArray:
	return ["zip"];

func _get_save_extension() -> String:
	return "tres";

func _get_resource_type() -> String:
	return "TextureSet";

func _get_preset_count() -> int:
	return 1;

func _get_preset_name(_preset_index: int) -> String:
	return "Default";

func _get_import_options(_path: String, _preset_index: int) -> Array:
	return [];

func _get_option_visibility(_path: String, _option_name: StringName, _options: Dictionary) -> bool:
	return true;

func _import(source_file: String, save_path: String, options: Dictionary, platform_variants: Array, gen_files: Array) -> Error:
	var global_path : String = ProjectSettings.globalize_path(source_file);
	var texture_set = TextureSetLoader.Load(global_path);
	var save_file = "%s.%s" % [save_path, _get_save_extension()];
	return ResourceSaver.save(texture_set, save_file);
