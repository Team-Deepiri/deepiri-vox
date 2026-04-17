extends Node

const MAX_ENEMIES := 15
const SPAWN_INTERVAL := 2.0
const MIN_INTERVAL := 0.5

var spawn_timer := 0.0
var difficulty := 1.0
var active_count := 0
var is_spawning := false

enum EnemyType { DRONE, FIGHTER, MOTHER }

func _ready():
	add_to_group("enemy_spawner")

func _process(delta):
	if not is_spawning or GameManager.state != GameManager.GameState.PLAYING:
		return
	
	spawn_timer -= delta
	if spawn_timer <= 0:
		spawn_enemy()
		spawn_timer = max(MIN_INTERVAL, SPAWN_INTERVAL - difficulty * 0.02)
		difficulty += 0.05

func start_spawning():
	is_spawning = true
	spawn_timer = 0.0
	difficulty = 1.0

func stop_spawning():
	is_spawning = false

func spawn_enemy():
	if active_count >= MAX_ENEMIES:
		return
	
	var etype = get_weighted_type()
	var enemy = load("res://scenes/enemy.tscn").instantiate()
	enemy.position = Vector2(randf_range(50, 750), -30)
	enemy.enemy_type = etype
	get_tree().current_scene.add_child(enemy)
	active_count += 1
	enemy.tree_exiting.connect(func(): active_count = max(0, active_count - 1))

func get_weighted_type() -> EnemyType:
	var r = randf()
	if difficulty < 3:
		return EnemyType.DRONE if r < 0.7 else EnemyType.FIGHTER if r < 0.95 else EnemyType.MOTHER
	elif difficulty < 6:
		return EnemyType.DRONE if r < 0.5 else EnemyType.FIGHTER if r < 0.85 else EnemyType.MOTHER
	else:
		return EnemyType.DRONE if r < 0.4 else EnemyType.FIGHTER if r < 0.7 else EnemyType.MOTHER