extends CharacterBody2D

const MOVE_SPEED := 300.0

var move_vel := Vector2.ZERO
var current_rocket: Node2D
var is_alive := true
var falling := false

@onready var body: Polygon2D = $Body
@onready var head: Polygon2D = $Head
@onready var fire_point: Marker2D = $FirePoint

func _ready():
	add_to_group("player")

func _physics_process(delta):
	if not is_alive:
		velocity = Vector2.ZERO
		move_and_slide()
		return
	
	if GameManager.state == GameManager.GameState.FALLING and falling:
		velocity = Vector2(0, 60)
		move_and_slide()
		return
	
	if GameManager.state != GameManager.GameState.PLAYING:
		velocity = Vector2.ZERO
		move_and_slide()
		return
	
	var input := Vector2.ZERO
	if Input.is_action_pressed("move_left"):
		input.x = -1
	elif Input.is_action_pressed("move_right"):
		input.x = 1
	
	if Input.is_action_pressed("move_up"):
		input.y = 1
	elif Input.is_action_pressed("move_down"):
		input.y = -1
	
	move_vel = input.normalized() * MOVE_SPEED
	velocity = move_vel
	move_and_slide()
	
	position.x = clamp(position.x, 40, 760)
	position.y = clamp(position.y, 200, 560)
	
	if velocity.x < 0:
		scale.x = -1
	elif velocity.x > 0:
		scale.x = 1
	
	if Input.is_action_pressed("fire") and fire_point:
		fire()

func fire():
	var bullet = load("res://scenes/bullet.tscn").instantiate()
	bullet.position = fire_point.global_position
	bullet.is_player_bullet = true
	get_tree().current_scene.add_child(bullet)

func mount_rocket(rocket_node):
	current_rocket = rocket_node
	falling = false
	if current_rocket:
		current_rocket.position = position + Vector2(0, -70)

func dismount_rocket():
	current_rocket = null
	falling = true

func die():
	if not is_alive:
		return
	is_alive = false
	visible = false
	GameManager.game_over()

func revive():
	is_alive = true
	visible = true
	falling = false
	position = Vector2(400, 480)

func _on_area_entered(area):
	if area.is_in_group("enemy") or area.is_in_group("enemy_bullet"):
		die()