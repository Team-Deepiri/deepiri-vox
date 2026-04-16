extends Area2D

@export var speed := 500.0
@export var damage := 1
@export var is_player_bullet := true

var velocity := Vector2.DOWN * speed

func _ready():
	if is_player_bullet:
		add_to_group("player_bullet")
	else:
		add_to_group("enemy_bullet")

func _physics_process(delta):
	position += velocity * delta
	
	if position.y < -20 or position.y > 620 or position.x < -20 or position.x > 820:
		queue_free()

func _on_area_entered(area):
	if is_player_bullet and area.is_in_group("enemy"):
		area.take_damage(damage)
		queue_free()
	elif not is_player_bullet and area.is_in_group("player"):
		area.die()
		queue_free()

func _on_timer_timeout():
	queue_free()