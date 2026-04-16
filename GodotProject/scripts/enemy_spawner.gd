extends Node

const SPAWN_INTERVAL := 2.0
const MIN_SPAWN_INTERVAL := 0.5
const MAX_ENEMIES := 20

var current_interval := SPAWN_INTERVAL
var last_spawn_time := 0.0
var difficulty := 1.0
var difficulty_rate := 0.1
var is_spawning := false
var active_count := 0

@onready var scene = get_tree().current_scene

func _ready():
	add_to_group("enemy_spawner")


func _process(delta):
	if not is_spawning or GameManager.game_state != GameManager.STATE_PLAYING:
		return
	
	var current_time = Time.get_ticks_msec() / 1000.0
	if current_time - last_spawn_time >= current_interval:
		spawn_enemy()
		last_spawn_time = current_time
	
	difficulty += difficulty_rate * delta
	current_interval = max(MIN_SPAWN_INTERVAL, SPAWN_INTERVAL - difficulty * 0.02)


func start_spawning():
	is_spawning = true
	difficulty = 1.0
	current_interval = SPAWN_INTERVAL
	last_spawn_time = 0.0


func stop_spawning():
	is_spawning = false


func spawn_enemy():
	if active_count >= MAX_ENEMIES:
		return
	
	var enemy_type = get_weighted_random()
	var enemy = preload("res://scenes/enemy.tscn").instantiate()
	enemy.enemy_type = enemy_type
	
	var spawn_x = randf_range(50, 750)
	enemy.position = Vector2(spawn_x, -30)
	
	scene.add_child(enemy)
	active_count += 1
	
	enemy.destroyed.connect(_on_enemy_destroyed)


func get_weighted_random() -> int:
	var r = randf()
	if difficulty < 3:
		if r < 0.7: return 0
		elif r < 0.95: return 1
		return 2
	elif difficulty < 6:
		if r < 0.5: return 0
		elif r < 0.85: return 1
		return 2
	else:
		if r < 0.4: return 0
		elif r < 0.7: return 1
		return 2


func _on_enemy_destroyed(_points):
	active_count = max(0, active_count - 1)