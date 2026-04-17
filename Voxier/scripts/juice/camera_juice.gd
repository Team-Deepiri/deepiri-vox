extends Camera2D

var _trauma := 0.0
const TRAUMA_DECAY := 1.85
const SHAKE_MULT := 22.0
const MENU_CENTER := Vector2(400, 300)
const FOLLOW_OFFSET := Vector2(0, -78.0)

func _ready() -> void:
	EventBus.camera_shake_requested.connect(_on_shake_requested)

func _exit_tree() -> void:
	if EventBus.camera_shake_requested.is_connected(_on_shake_requested):
		EventBus.camera_shake_requested.disconnect(_on_shake_requested)

func _on_shake_requested(amount: float) -> void:
	add_trauma(amount)

func add_trauma(amount: float) -> void:
	_trauma = clampf(_trauma + amount, 0.0, 1.0)

func _process(delta: float) -> void:
	var st := GameManager.state
	var follow := MENU_CENTER
	if st == GameManager.GameState.PLAYING or st == GameManager.GameState.FALLING:
		var pl := GameManager.player
		if pl and is_instance_valid(pl):
			follow = pl.global_position + FOLLOW_OFFSET
	position = position.lerp(follow, 1.0 - exp(-4.25 * delta))

	if _trauma <= 0.0:
		offset = Vector2.ZERO
		return
	_trauma = maxf(0.0, _trauma - TRAUMA_DECAY * delta)
	var shake := _trauma * _trauma * SHAKE_MULT
	offset = Vector2(randf_range(-1.0, 1.0), randf_range(-1.0, 1.0)) * shake
