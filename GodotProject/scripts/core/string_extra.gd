extends RefCounted

static func repeat_line(char: String, length: int) -> String:
	return char.repeat(max(0, length))

static func pad_right(s: String, width: int) -> String:
	var out := s
	while out.length() < width:
		out += " "
	return out
