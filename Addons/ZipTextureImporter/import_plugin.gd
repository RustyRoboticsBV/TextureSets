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
	"BPTC",
	"BPTC",
	"BPTC",
	"S3TC",
	"BPTC",
	"BPTC",
	"BPTC",
	"BPTC",
	"BPTC"
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
	var error : Error = Error.OK;
	
	# Open ZIP file.
	var reader : ZIPReader = ZIPReader.new();
	error = reader.open(source_file);
	if error != Error.OK:
		push_error("Could not open ZIP file at: " + source_file);
	
	# Find src images in ZIP.
	var src_r : String = "";
	var src_g : String = "";
	var src_b : String = "";
	var src_a : String = "";
	var files : PackedStringArray = reader.get_files();
	for file : String in files:
		var ext : String = file.get_extension();
		var trimmed = file.trim_suffix('.' + ext);
		if file == options.r_channel_source || trimmed == options.r_channel_source:
			src_r = file;
		if file == options.g_channel_source || trimmed == options.g_channel_source:
			src_g = file;
		if file == options.b_channel_source || trimmed == options.b_channel_source:
			src_b = file;
		if file == options.a_channel_source || trimmed == options.a_channel_source:
			src_a = file;
	
	# Load source images.
	if src_r == "" && options.r_channel_source != "":
		push_error("Cannot read red channel image: " + options.r_channel_source);
	var img_r : Image = load_image(reader, src_r);
	
	if src_g == "" && options.g_channel_source != "":
		push_error("Cannot read green channel image: " + options.g_channel_source);
	var img_g : Image = load_image(reader, src_g);
	
	if src_b == "" && options.b_channel_source != "":
		push_error("Cannot read blue channel image: " + options.b_channel_source);
	var img_b : Image = load_image(reader, src_b);
	
	if src_a == "" && options.a_channel_source != "":
		push_error("Cannot read alpha channel image: " + options.a_channel_source);
	var img_a : Image = load_image(reader, src_a);
	
	# Pack channels and create texture.
	var texture : ImageTexture = pack_texture(img_r, img_g, img_b, img_a);
	
	if texture == null:
		var img : Image = Image.create(1, 1, false, Image.FORMAT_RGBA8);
		img.set_pixel(0, 0, Color(0, 0, 0, 1));
		
		texture = ImageTexture.create_from_image(img);
	
	# Apply settings.
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

func load_image(zip_reader : ZIPReader, path : String) -> Image:
	if path.is_empty():
		return null;
	
	# Read bytes from ZIP.
	var bytes : PackedByteArray = zip_reader.read_file(path);
	
	# Load as image.
	var img : Image = Image.new();
	var ext = path.get_extension().to_lower();
	match ext:
		"bmp":
			img.load_bmp_from_buffer(bytes);
		"jpg":
			img.load_jpg_from_buffer(bytes);
		"jpeg":
			img.load_jpg_from_buffer(bytes);
		"png":
			img.load_png_from_buffer(bytes);
		"svg":
			img.load_svg_from_buffer(bytes);
		"tga":
			img.load_tga_from_buffer(bytes);
		"webp":
			img.load_webp_from_buffer(bytes);
		"dds":
			img.load_dds_from_buffer(bytes);
		"ktx":
			img.load_ktx_from_buffer(bytes);
		"_":
			push_error("Unsupported file type: ." + ext);
	return img;

func pack_texture(img_r : Image, img_g : Image, img_b : Image, img_a : Image) -> ImageTexture:
	# Pick first available image as base.
	var base_img : Image = null;
	for img in [img_r, img_g, img_b, img_a]:
		if img != null and not img.is_empty():
			base_img = img;
			break;
	
	if base_img == null:
		return null;
	
	# Get dimensions.
	var width = base_img.get_width()
	var height = base_img.get_height()
	
	# Ensure all images match size.
	for img in [img_r, img_g, img_b, img_a]:
		if img != null and not img.is_empty():
			if img.get_width() != width or img.get_height() != height:
				img.resize(width, height, Image.INTERPOLATE_BILINEAR);
	
	# Create final image.
	var final_img : Image = Image.create(width, height, false, Image.FORMAT_RGBA8)
	
	for y in height:
		for x in width:
			var r = img_r.get_pixel(x, y).r if img_r and not img_r.is_empty() else 0.0;
			var g = img_g.get_pixel(x, y).g if img_g and not img_g.is_empty() else 0.0;
			var b = img_b.get_pixel(x, y).b if img_b and not img_b.is_empty() else 0.0;
			var a = img_a.get_pixel(x, y).a if img_a and not img_a.is_empty() else 1.0;
			
			final_img.set_pixel(x, y, Color(r, g, b, a));
	
	# Create texture.
	return ImageTexture.create_from_image(final_img);
