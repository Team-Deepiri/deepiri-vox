extends Area2D

@export var speed := 520.0
@export var damage := 1
@export var is_player_bullet := true

var velocity := Vector2.ZERO

func _ready() -> void:
	if is_player_bullet:
		add_to_group("player_bullet")
		velocity = Vector2.UP * speed
	else:
		add_to_group("enemy_bullet")
		velocity = Vector2.DOWN * speed
	var lt := get_node_or_null("Lifetime") as Timer
	if lt:
		lt.start()

func _physics_process(delta: float) -> void:
	position += velocity * delta
	if position.y < -20 or position.y > 620 or position.x < -20 or position.x > 820:
		queue_free()

func _on_area_entered(area: Area2D) -> void:
	if is_player_bullet and area.is_in_group("enemy"):
		area.take_damage(damage)
		queue_free()
	elif not is_player_bullet and area.get_parent() is CharacterBody2D:
		var body := area.get_parent() as CharacterBody2D
		if body.is_in_group("player"):
			queue_free()

func _on_timer_timeout() -> void:
	queue_free()
