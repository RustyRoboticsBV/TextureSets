@tool
extends EditorPlugin

var importer : EditorImportPlugin;

func _enter_tree():
	importer = preload("res://Addons/ZipTextureImporter/import_plugin.gd").new();
	add_import_plugin(importer)

func _exit_tree():
	remove_import_plugin(importer)
