extends RigidBody3D


#This class is for the silly background in the main menu.

# Called when the node enters the scene tree for the first time.
func _ready():
	
	inertia = Vector3.ZERO
	
	#apply_torque_impulse(randomVector3(50000000,10000000))
	#apply_central_impulse(randomVector3(50000000,10000000))
	position = randomVector3(15,5)
	pass # Replace with function body.

var lastImpulse = 0
var timeToChange = 0

func _physics_process(delta):
	
	linear_velocity += -position * 6 * delta
	
	if Time.get_ticks_msec() > lastImpulse + timeToChange:
		timeToChange = randf_range(300,800)
		lastImpulse = Time.get_ticks_msec()
		apply_central_impulse(randomVector3(50,10))
		apply_torque_impulse(randomVector3(8,1))
	
	pass

func randomVector3(maxLength: float,minLength : float = 0.0) -> Vector3:
	var x = randf_range(-1.0, 1.0)
	var y = randf_range(-1.0, 1.0)
	var z = randf_range(-1.0, 1.0)
	
	var vec = Vector3(x, y, z).normalized() * randf_range(minLength, maxLength)
	#print(vec)
	return vec
