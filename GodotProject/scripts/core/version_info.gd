extends RefCounted

static func engine_label() -> String:
	return "Godot %d.%d" % [Engine.get_version_info().major, Engine.get_version_info().minor]
