extends CanvasLayer

var _label: Label

func _ready() -> void:
	visible = OS.is_debug_build()
	if not visible:
		return
	_label = Label.new()
	_label.position = Vector2(8, 8)
	_label.add_theme_color_override("font_color", Color(0.5, 1.0, 0.5, 0.85))
	add_child(_label)

func _process(_delta: float) -> void:
	if _label:
		_label.text = "FPS %d" % Engine.get_frames_per_second()
