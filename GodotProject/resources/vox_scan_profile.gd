extends Resource
class_name VoxScanProfile

@export var name_prefixes: PackedStringArray = PackedStringArray(["deepiri-", "diri-"])
@export var skip_directory_names: PackedStringArray = PackedStringArray([".git", "node_modules", ".godot", "__pycache__"])
