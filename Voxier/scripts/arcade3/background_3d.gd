extends Node3D

var _stars: Node3D


func _ready() -> void:
	add_to_group("background_manager")
	_stars = Node3D.new()
	_stars.name = "Stars"
	add_child(_stars)
	for i in range(120):
		var m := MeshInstance3D.new()
		var s := SphereMesh.new()
		s.radius = randf_range(0.02, 0.07)
		s.height = s.radius * 2.0
		m.mesh = s
		var mat := StandardMaterial3D.new()
		mat.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
		mat.albedo_color = Color(1, 1, 1, randf_range(0.35, 0.95))
		m.material_override = mat
		m.position = Vector3(randf_range(-18.0, 18.0), randf_range(2.0, 14.0), randf_range(-6.0, 42.0))
		_stars.add_child(m)


func _process(delta: float) -> void:
	if GameManager.state != GameManager.GameState.PLAYING and GameManager.state != GameManager.GameState.FALLING:
		return
	var mul := 1.0
	if GameManager.state == GameManager.GameState.FALLING:
		mul = 2.2
	for c in _stars.get_children():
		if c is Node3D:
			c.position.z += delta * randf_range(2.0, 9.0) * mul * 0.35
			if c.position.z > 48.0:
				c.position.z = -8.0
				c.position.x = randf_range(-18.0, 18.0)


func set_background(_type: int) -> void:
	pass


func apply_shift(shift: Vector2) -> void:
	position.x = shift.x * 0.025


func cycle_background() -> void:
	var env := get_parent().get_parent().get_node_or_null("WorldEnvironment") as WorldEnvironment
	if env and env.environment:
		var e := env.environment
		e.ambient_light_color = Color(randf(), randf(), randf()).lerp(Color(0.2, 0.25, 0.45), 0.65)
