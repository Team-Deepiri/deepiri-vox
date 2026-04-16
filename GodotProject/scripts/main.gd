extends Node2D

@onready var player: CharacterBody2D = $Player
@onready var rocket: Node2D = $Rocket
@onready var bg_manager: Node2D = $BackgroundManager
@onready var dir_controller: Node = $DirectionController
@onready var spawner: Node = $EnemySpawner
@onready var camera: Camera2D = $Camera2D
@onready var start_btn: Button = $"%StartBtn"
@onready var retry_btn: Button = $"%RetryBtn"

func _ready():
	player.add_to_group("player")
	rocket.add_to_group("rocket")
	bg_manager.add_to_group("background_manager")
	dir_controller.add_to_group("direction_controller")
	spawner.add_to_group("enemy_spawner")
	start_btn.pressed.connect(_on_start_pressed)
	retry_btn.pressed.connect(_on_retry_pressed)

func _on_start_pressed():
	GameManager.start_game()

func _on_retry_pressed():
	GameManager.restart()

func _input(event):
	if event.is_action_pressed("fire") and GameManager.state == GameManager.GameState.PLAYING:
		pass
	
	if event.is_action_pressed("hop") or event.is_action_pressed("ui_accept"):
		if GameManager.state == GameManager.ROCKET_CHANGE:
			GameManager.hop_to_rocket()