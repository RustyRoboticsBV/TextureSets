@tool
extends EditorImportPlugin;

const FormatType = "format/type";
const FormatCompression = "format/compression";
const FormatUseMipmaps = "format/use_mipmaps";

const RedSourceImage = "red/source_image";
const RedSourceChannel = "red/source_channel";
const RedInvert = "red/invert";

const GreenSourceImage = "green/source_image";
const GreenSourceChannel = "green/source_channel";
const GreenInvert = "green/invert";

const BlueSourceImage = "blue/source_image";
const BlueSourceChannel = "blue/source_channel";
const BlueInvert = "blue/invert";

const AlphaSourceImage = "alpha/source_image";
const AlphaSourceChannel = "alpha/source_channel";
const AlphaInvert = "alpha/invert";

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

const CompressionSource = [
	"sRGB",
	"sRGB",
	"sRGB",
	"Normal Map",
	"Linear",
	"Linear",
	"Linear",
	"Linear",
	"sRGB"
	
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
			"name": FormatType,
			"default_value": CompressionSource[preset_index],
			"property_hint": PROPERTY_HINT_ENUM,
			"hint_string": "Linear,sRGB,Normal Map"
		},
		{
			"name": FormatCompression,
			"default_value": Compression[preset_index],
			"property_hint": PROPERTY_HINT_ENUM,
			"hint_string": "None,ASTC (4x4),ASTC (8x8),BPTC,ETC,ETC2,S3TC"
		},
		{
			"name": FormatUseMipmaps,
			"default_value": true
		},
		
		{
			"name": RedSourceImage,
			"default_value": Red[preset_index]
		},
		{
			"name": RedSourceChannel,
			"default_value": "Red",
			"property_hint": PROPERTY_HINT_ENUM,
			"hint_string": "Red,Green,Blue,Alpha"
		},
		{
			"name": RedInvert,
			"default_value": false
		},
		
		{
			"name": GreenSourceImage,
			"default_value": Green[preset_index]
		},
		{
			"name": GreenSourceChannel,
			"default_value": "Green",
			"property_hint": PROPERTY_HINT_ENUM,
			"hint_string": "Red,Green,Blue,Alpha"
		},
		{
			"name": GreenInvert,
			"default_value": false
		},
		
		{
			"name": BlueSourceImage,
			"default_value": Blue[preset_index]
		},
		{
			"name": BlueSourceChannel,
			"default_value": "Blue",
			"property_hint": PROPERTY_HINT_ENUM,
			"hint_string": "Red,Green,Blue,Alpha"
		},
		{
			"name": BlueInvert,
			"default_value": false
		},
		
		{
			"name": AlphaSourceImage,
			"default_value": Alpha[preset_index]
		},
		{
			"name": AlphaSourceChannel,
			"default_value": "Alpha",
			"property_hint": PROPERTY_HINT_ENUM,
			"hint_string": "Red,Green,Blue,Alpha"
		},
		{
			"name": AlphaInvert,
			"default_value": false
		}
	];

func _get_option_visibility(_path: String, _option_name: StringName, _options: Dictionary) -> bool:
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
		if file == options[RedSourceImage] || trimmed == options[RedSourceImage]:
			src_r = file;
		if file == options[GreenSourceImage] || trimmed == options[GreenSourceImage]:
			src_g = file;
		if file == options[BlueSourceImage] || trimmed == options[BlueSourceImage]:
			src_b = file;
		if file == options[AlphaSourceImage] || trimmed == options[AlphaSourceImage]:
			src_a = file;
	
	# Load source images.
	if src_r == "" && options[RedSourceImage] != "":
		push_error("Cannot read red channel image: " + options[RedSourceImage]);
	var img_r : Image = load_image(reader, src_r);
	
	if src_g == "" && options[GreenSourceImage] != "":
		push_error("Cannot read green channel image: " + options[GreenSourceImage]);
	var img_g : Image = load_image(reader, src_g);
	
	if src_b == "" && options[BlueSourceImage] != "":
		push_error("Cannot read blue channel image: " + options[BlueSourceImage]);
	var img_b : Image = load_image(reader, src_b);
	
	if src_a == "" && options[AlphaSourceImage] != "":
		push_error("Cannot read alpha channel image: " + options[AlphaSourceImage]);
	var img_a : Image = load_image(reader, src_a);
	
	# Pack channels and create texture.
	var img : Image = pack_image(img_r, img_g, img_b, img_a);
	
	# Apply inversions.
	invert_image_channel(img, options[RedInvert], options[GreenInvert], options[BlueInvert], options[AlphaInvert]);
	
	# Mipmaps.
	if options[FormatUseMipmaps]:
		if options[FormatType] == "Normal Map":
			img.generate_mipmaps(true);
		else:
			img.generate_mipmaps(false);
	
	# Compression.
	var compress_source : Image.CompressSource = Image.CompressSource.COMPRESS_SOURCE_GENERIC;
	match options[FormatType]:
		"Linear":
			compress_source = Image.CompressSource.COMPRESS_SOURCE_GENERIC;
		"sRGB":
			compress_source = Image.CompressSource.COMPRESS_SOURCE_SRGB;
		"Normal Map":
			compress_source = Image.CompressSource.COMPRESS_SOURCE_NORMAL;
			
	match options[FormatCompression]:
		"ASTC (4x4)":
			img.compress(Image.CompressMode.COMPRESS_ASTC, compress_source, Image.ASTCFormat.ASTC_FORMAT_4x4);
		"ASTC (8x8)":
			img.compress(Image.CompressMode.COMPRESS_ASTC, compress_source, Image.ASTCFormat.ASTC_FORMAT_8x8);
		"BPTC":
			img.compress(Image.CompressMode.COMPRESS_BPTC, compress_source);
		"ETC":
			img.compress(Image.CompressMode.COMPRESS_ETC, compress_source);
		"ETC2":
			img.compress(Image.CompressMode.COMPRESS_ETC2, compress_source);
		"S3TC":
			img.compress(Image.CompressMode.COMPRESS_S3TC, compress_source);
	
	# Create texture.
	var texture : ImageTexture = ImageTexture.create_from_image(img);
	
	# Save file.
	var save_file : String = "%s.%s" % [save_path, _get_save_extension()];
	return ResourceSaver.save(texture, save_file);

# Loads an image from a ZIP file.
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

# Packs four images together.
func pack_image(img_r : Image, img_g : Image, img_b : Image, img_a : Image) -> Image:
	# Pick first available image as base.
	var base_img : Image = null;
	for img in [img_r, img_g, img_b, img_a]:
		if img != null and not img.is_empty():
			base_img = img;
			break;
	
	# Handle empty images.
	if base_img == null:
		var empty_img : Image = Image.create(1, 1, false, Image.FORMAT_RGBA8);
		empty_img.set_pixel(0, 0, Color(0, 0, 0, 1));
		return empty_img;
	
	# Get dimensions.
	var width = base_img.get_width();
	var height = base_img.get_height();
	
	# Ensure all images match size.
	for img in [img_r, img_g, img_b, img_a]:
		if img != null and not img.is_empty():
			if img.get_width() != width or img.get_height() != height:
				img.resize(width, height, Image.INTERPOLATE_BILINEAR);
	
	# Create final image.
	var final_img : Image = Image.create(width, height, false, Image.FORMAT_RGBA8);
	
	for y in height:
		for x in width:
			var r = img_r.get_pixel(x, y).r if img_r and not img_r.is_empty() else 0.0;
			var g = img_g.get_pixel(x, y).g if img_g and not img_g.is_empty() else 0.0;
			var b = img_b.get_pixel(x, y).b if img_b and not img_b.is_empty() else 0.0;
			var a = img_a.get_pixel(x, y).a if img_a and not img_a.is_empty() else 1.0;
			
			final_img.set_pixel(x, y, Color(r, g, b, a));
	
	return final_img;

# Invert one or more channels.
func invert_image_channel(img: Image, r : bool, g : bool, b : bool, a : bool) -> void:
	if !r && !g && !b && !a:
		return;
	
	for y in img.get_height():
		for x in img.get_width():
			var pixel = img.get_pixel(x, y);

			if r:
				pixel.r = 1.0 - pixel.r;
			if g:
				pixel.g = 1.0 - pixel.g;
			if b:
				pixel.b = 1.0 - pixel.b;
			if a:
				pixel.a = 1.0 - pixel.a;

			img.set_pixel(x, y, pixel);
