extends Skeleton3D


# Called when the node enters the scene tree for the first time.
func _ready():
	
	$"FR IK".start();
	$"FL IK".start();
	$"BR IK".start();
	$"BL IK".start();
	
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	operateLegs($"FR Cast",$"FR target")
	operateLegs($"FL Cast",$"FL target")
	operateLegs($"BR Cast",$"BR target")
	operateLegs($"BL Cast",$"BL target")
	
	
	pass

var stepLength = 1
var stepHeight = 1

var stepDuration = 0.5

func operateLegs(raycast : RayCast3D, target : Marker3D):
	
	if raycast.is_colliding() and raycast.get_collision_point().distance_to(target.global_position > stepLength):
		
		var tween = get_tree().create_tween();
		
		var newLegPos = raycast.get_collision_point()
		var midPos = (target.global_position + newLegPos)/2 + stepHeight * Vector3.UP
		
		tween.tween_property(target,"global_position",midPos,stepLength/2)
		tween.tween_property(target,"global_position",newLegPos,stepLength/2)
		
	
	pass
