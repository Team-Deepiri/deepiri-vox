extends Node3D

@export var target_path: NodePath = NodePath()


func _process(_delta: float) -> void:
	var tgt := get_node_or_null(target_path) as Node3D
	if tgt:
		look_at(tgt.global_position + Vector3(0, 0.75, 0), Vector3.UP)
