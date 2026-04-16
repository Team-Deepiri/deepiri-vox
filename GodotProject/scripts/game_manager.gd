extends Node

enum GameState { MENU, PLAYING, PAUSED, GAME_OVER, FALLING, ROCKET_CHANGE }

var state := GameState.MENU
var score := 0
var lives := 3
var current_rocket: Node2D
var new_rocket: Node2D
var has_new_rocket := false
var rocket_timer := 25.0
var rocket_interval := 25.0
var min_interval := 8.0
var fall_height := 0.0
var max_fall_height := 200.0

@onready var ui_score: Label = $"%ScoreLabel"
@onready var ui_lives: Label = $"%LivesLabel"
@onready var ui_timer: Label = $"%TimerLabel"
@onready var ui_direction: Label = $"%DirectionLabel"
@onready var ui_rocket_warning: Control = $"%RocketWarning"
@onready var ui_start: Control = $"%StartPanel"
@onready var ui_gameover: Control = $"%GameOverPanel"
@onready var ui_finalscore: Label = $"%FinalScoreLabel"

var player: CharacterBody2D
var direction_controller: Node
var background_manager: Node
var enemy_spawner: Node

func _ready():
	player = get_tree().get_first_node_in_group("player")
	direction_controller = get_tree().get_first_node_in_group("direction_controller")
	background_manager = get_tree().get_first_node_in_group("background_manager")
	enemy_spawner = get_tree().get_first_node_in_group("enemy_spawner")

func _process(delta):
	if state != GameState.PLAYING and state != GameState.FALLING:
		return
	
	update_ui()
	
	if state == GameState.FALLING:
		handle_falling(delta)
	elif state == GameState.ROCKET_CHANGE:
		rocket_timer -= delta
		if rocket_timer <= 0:
			missed_rocket()

func start_game():
	state = GameState.PLAYING
	score = 0
	lives = 3
	has_new_rocket = false
	rocket_interval = 25.0
	rocket_timer = 0.0
	fall_height = 0.0
	
	ui_start.visible = false
	ui_gameover.visible = false
	
	if player:
		player.revive()
		player.visible = true
		player.falling = false
		player.position = Vector2(400, 480)
	
	current_rocket = get_tree().get_first_node_in_group("rocket")
	if current_rocket:
		current_rocket.activate()
		current_rocket.position = Vector2(400, 400)
		player.mount_rocket(current_rocket)
	
	if enemy_spawner:
		enemy_spawner.start_spawning()
	
	if direction_controller:
		direction_controller.reset()
	
	if background_manager:
		background_manager.set_background(0)

func game_over():
	state = GameState.GAME_OVER
	ui_finalscore.text = "SCORE: " + str(score)
	ui_gameover.visible = true
	
	if enemy_spawner:
		enemy_spawner.stop_spawning()

func on_rocket_exploded():
	has_new_rocket = true
	rocket_timer = 3.0
	ui_rocket_warning.visible = true
	
	if player:
		player.dismount_rocket()
		player.falling = true
		state = GameState.FALLING
		fall_height = 0.0

func handle_falling(delta):
	fall_height += 80 * delta
	
	if player:
		player.position.y += 60 * delta
	
	if has_new_rocket and new_rocket:
		var dist = player.position.distance_to(new_rocket.position)
		if dist < 40:
			catch_rocket()
			return
	
	if fall_height > max_fall_height:
		lose_life()

func catch_rocket():
	var damage = int(fall_height / 30)
	lives -= damage
	
	current_rocket = new_rocket
	current_rocket.activate()
	current_rocket.position = player.position + Vector2(0, 70)
	
	player.mount_rocket(current_rocket)
	player.falling = false
	has_new_rocket = false
	new_rocket = null
	
	rocket_interval = max(min_interval, rocket_interval - 2.0)
	rocket_timer = rocket_interval
	
	ui_rocket_warning.visible = false
	
	state = GameState.PLAYING
	
	if lives <= 0:
		game_over()

func lose_life():
	lives -= 1
	
	if has_new_rocket and new_rocket:
		new_rocket.queue_free()
	new_rocket = null
	
	has_new_rocket = false
	ui_rocket_warning.visible = false
	
	if lives <= 0:
		state = GameState.GAME_OVER
		ui_finalscore.text = "SCORE: " + str(score)
		ui_gameover.visible = true
	else:
		start_game()

func hop_to_rocket():
	if not has_new_rocket or not new_rocket:
		return
	
	if current_rocket:
		current_rocket.deactivate()
		current_rocket.queue_free()
	
	current_rocket = new_rocket
	current_rocket.activate()
	current_rocket.position = player.position + Vector2(0, 70)
	
	player.mount_rocket(current_rocket)
	has_new_rocket = false
	new_rocket = null
	rocket_timer = 0.0
	
	rocket_interval = max(min_interval, rocket_interval - 1.0)
	rocket_timer = rocket_interval
	
	ui_rocket_warning.visible = false
	
	state = GameState.PLAYING
	
	if background_manager:
		background_manager.cycle_background()

func missed_rocket():
	if new_rocket:
		new_rocket.queue_free()
	lose_life()

func add_score(points: int):
	score += points

func update_ui():
	if ui_score:
		ui_score.text = str(score)
	if ui_lives:
		ui_lives.text = "LIVES: " + str(lives)
	if state == GameState.PLAYING and ui_timer:
		ui_timer.text = str(ceil(rocket_timer))
	if direction_controller and ui_direction:
		ui_direction.text = direction_controller.get_direction_name()

func restart():
	get_tree().reload_current_scene()