extends Skeleton3D

var legs = []

# Called when the node enters the scene tree for the first time.
func _ready():
	
	$"FR IK".start();
	$"FL IK".start();
	$"BR IK".start();
	$"BL IK".start();
	
	
	
	legs.append(BossLeg.new($"FL Cast",$"FL target",legs))
	legs.append(BossLeg.new($"BR Cast",$"BR target",legs))
	legs[0].setBuddy(legs[1])
	
	legs.append(BossLeg.new($"FR Cast",$"FR target",legs))
	legs.append(BossLeg.new($"BL Cast",$"BL target",legs))
	legs[2].setBuddy(legs[3])
	
	pass # Replace with function body.


var velocity = Vector3.ZERO
var lastPosition = Vector3.ZERO
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	var thisFrameVelocity = (global_position - lastPosition) * 1.0/delta  
	lastPosition = global_position
	velocity = (velocity * 9 + thisFrameVelocity)/10
	
	velocity.y = 0;
	
	for leg in legs:
		leg.operate(get_tree(),velocity)
	
	pass





