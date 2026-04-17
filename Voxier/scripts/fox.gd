extends CharacterBody2D

const MOVE_SPEED := 300.0
const FIRE_COOLDOWN := 0.15
const FOX_SIZE := 40

@onready var body_sprite := $BodySprite
@onready var head_sprite := $HeadSprite
@onready var tail_sprite := $TailSprite
@onready var backpack_sprite := $BackpackSprite
@onready var shirt_sprite := $ShirtSprite
@onready var fire_point := $FirePoint

var current_rocket : Node2D = null
var is_alive := true
var last_fire_time := 0.0
var can_shoot := true

signal died


func _ready():
	add_to_group("player")


func _physics_process(delta):
	if not is_alive or GameManager.game_state != GameManager.STATE_PLAYING:
		velocity = Vector2.ZERO
		return
	
	handle_input()
	handle_movement(delta)
	update_animation()


func handle_input():
	velocity = Vector2.ZERO
	
	if Input.is_action_pressed("move_left"):
		velocity.x = -MOVE_SPEED
	elif Input.is_action_pressed("move_right"):
		velocity.x = MOVE_SPEED
	
	if Input.is_action_pressed("move_up"):
		velocity.y = MOVE_SPEED
	elif Input.is_action_pressed("move_down"):
		velocity.y = -MOVE_SPEED
	
	velocity = velocity.normalized() * MOVE_SPEED
	
	if Input.is_action_just_pressed("fire") and can_shoot:
		fire()


func fire():
	if Time.get_ticks_msec() / 1000.0 - last_fire_time >= FIRE_COOLDOWN:
		var bullet = preload("res://scenes/bullet.tscn").instantiate()
		bullet.position = fire_point.global_position
		bullet.is_player_bullet = true
		get_tree().current_scene.add_child(bullet)
		last_fire_time = Time.get_ticks_msec() / 1000.0


func handle_movement(delta):
	move_and_slide()
	
	position.x = clamp(position.x, FOX_SIZE / 2, 800 - FOX_SIZE / 2)
	position.y = clamp(position.y, 150 + FOX_SIZE / 2, 600 - FOX_SIZE / 2)
	
	if current_rocket:
		current_rocket.position = position + Vector2(0, -70)


func update_animation():
	if velocity.x < 0:
		scale.x = -1
	elif velocity.x > 0:
		scale.x = 1


func mount_rocket(rocket_node):
	current_rocket = rocket_node


func dismount_rocket():
	current_rocket = null


func die():
	if not is_alive:
		return
	is_alive = false
	visible = false
	died.emit()
	GameManager.game_over()


func revive():
	is_alive = true
	visible = true


func _on_area_entered(area):
	if area.is_in_group("enemy") or area.is_in_group("enemy_bullet"):
		die()