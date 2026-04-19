# addons/your_plugin/plugin.gd
@tool
extends EditorPlugin

var importer : EditorImportPlugin;

func _enter_tree():
	importer = preload("res://Addons/TextureSetImporter/import_plugin.gd").new();
	add_import_plugin(importer)

func _exit_tree():
	remove_import_plugin(importer)
