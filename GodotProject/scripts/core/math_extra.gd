extends RefCounted

static func approach(from: float, to: float, delta: float) -> float:
	if from < to:
		return min(from + delta, to)
	return max(from - delta, to)

static func smooth_damp(current: float, target: float, velocity_ref: float, smooth_time: float, delta: float) -> float:
	if smooth_time < 0.00001:
		return target
	var omega := 2.0 / smooth_time
	var x := omega * delta
	var exp := 1.0 / (1.0 + x + 0.48 * x * x + 0.235 * x * x * x)
	var change := current - target
	var temp := (velocity_ref + omega * change) * delta
	velocity_ref = (velocity_ref - omega * temp) * exp
	return target + (change + temp) * exp
