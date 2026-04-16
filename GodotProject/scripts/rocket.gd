extends Node2D

const MAX_TIME := 25.0

var current_timer := MAX_TIME
var exploding := false
var start_y := 0.0

@onready var body: Polygon2D = $Body
@onready var flame: CPUParticles2D = $Flame

func _ready():
	add_to_group("rocket")
	start_y = position.y
	current_timer = MAX_TIME

func _process(delta):
	if GameManager.state != GameManager.GameState.PLAYING:
		if flame:
			flame.emitting = false
		return
	
	if exploding:
		current_timer -= delta
		if current_timer < -2:
			rocket_dead()
		return
	
	GameManager.rocket_timer -= delta
	
	current_timer = GameManager.rocket_timer
	position.y = start_y + sin(Time.get_ticks_msec() / 1000.0 * 3) * 3
	
	if flame:
		flame.emitting = true
	
	if current_timer <= 0:
		explode()

func explode():
	exploding = true
	visible = false
	if flame:
		flame.emitting = false
	
	current_timer = 2.0
	GameManager.on_rocket_exploded()

func spawn_new() -> Node2D:
	var new_r = load("res://scenes/rocket.tscn").instantiate()
	new_r.position = Vector2(randf_range(100, 700), randf_range(100, 300))
	get_tree().current_scene.add_child(new_r)
	GameManager.new_rocket = new_r
	return new_r

func activate():
	exploding = false
	visible = true
	current_timer = MAX_TIME
	GameManager.rocket_timer = MAX_TIME
	GameManager.rocket_interval = max(8.0, GameManager.rocket_interval - 2.0)
	if flame:
		flame.emitting = true

func deactivate():
	if flame:
		flame.emitting = false

func rocket_dead():
	pass