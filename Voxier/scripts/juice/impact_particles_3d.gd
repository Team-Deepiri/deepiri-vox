extends RefCounted

static func burst(parent: Node3D, global_pos: Vector3, color: Color, amount: int = 32) -> void:
	if parent == null:
		return
	var p := CPUParticles3D.new()
	p.one_shot = true
	p.emitting = false
	p.explosiveness = 0.92
	p.amount = amount
	p.lifetime = 0.42
	p.direction = Vector3(0, 1, 0)
	p.spread = 180.0
	p.initial_velocity_min = 2.5
	p.initial_velocity_max = 8.5
	p.gravity = Vector3(0, -10.0, 0)
	p.scale_amount_min = 0.08
	p.scale_amount_max = 0.28
	p.color = color
	parent.add_child(p)
	p.global_position = global_pos
	p.emitting = true
	var tw := parent.get_tree().create_timer(p.lifetime + 0.2)
	tw.timeout.connect(func(): p.queue_free())
