extends RefCounted

static func play_one_shot(stream: AudioStream, volume_db: float = 0.0) -> void:
	if stream == null:
		return
	var tree := Engine.get_main_loop() as SceneTree
	if tree == null:
		return
	var p := AudioStreamPlayer.new()
	p.stream = stream
	p.volume_db = volume_db
	p.bus = "Master"
	tree.root.add_child(p)
	p.finished.connect(p.queue_free)
	p.play()
