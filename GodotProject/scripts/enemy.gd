extends Area2D

enum EnemyType { DRONE, FIGHTER, MOTHER }

@export var enemy_type := EnemyType.DRONE

var health := 1
var score_value := 100
var move_speed := 150.0
var fire_rate := 0.5
var zigzag := false

@onready var player = get_tree().get_first_node_in_group("player")

func _ready():
	add_to_group("enemy")
	setup_enemy()

func setup_enemy():
	match enemy_type:
		EnemyType.DRONE:
			health = 1
			score_value = 100
			move_speed = 150
			fire_rate = 0.5
			zigzag = true
		EnemyType.FIGHTER:
			health = 2
			score_value = 250
			move_speed = 200
			fire_rate = 0.8
		EnemyType.MOTHER:
			health = 10
			score_value = 1000
			move_speed = 50
			fire_rate = 1.5
			scale = Vector2(1.5, 1.5)

func _physics_process(delta):
	if GameManager.state != GameManager.GameState.PLAYING:
		return
	
	position.y -= move_speed * delta
	
	if zigzag:
		position.x += sin(Time.get_ticks_msec() / 1000.0 * 3) * 50 * delta
	
	if player:
		var direction = (player.position - position).normalized()
		if enemy_type == EnemyType.FIGHTER:
			position.x += direction.x * move_speed * 0.3 * delta
	
	if position.y > 620:
		queue_free()

func take_damage(dmg: int):
	health -= dmg
	if health <= 0:
		die()

func die():
	GameManager.add_score(score_value)
	if enemy_type == EnemyType.MOTHER and randf() < 0.3:
		spawn_powerup()
	queue_free()

func spawn_powerup():
	var powerup = load("res://scenes/powerup.tscn").instantiate()
	powerup.position = position
	get_tree().current_scene.add_child(powerup)

func _on_area_entered(area):
	if area.is_in_group("player_bullet"):
		take_damage(area.damage)
		area.queue_free()

func _on_timer_timeout():
	queue_free()