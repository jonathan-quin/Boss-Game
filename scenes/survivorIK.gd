extends Skeleton3D

var legs = []

# Called when the node enters the scene tree for the first time.
func _ready():
	
	$"Right arm IK".start();
	
	$"R leg IK".start();
	$"L leg IK".start();
	
	
	
	legs.append(BossLeg.new($"L leg cast",$"L leg target",legs,1,0.5,0.3,3,0.1))
	legs.append(BossLeg.new($"R leg cast",$"R leg target",legs,1,0.5,0.3,3,0.1))
	#legs[0].setBuddy(legs[1])
	
	
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

