extends Skeleton3D

var legs = []

# Called when the node enters the scene tree for the first time.
func _ready():
	
	$"FR IK".start();
	$"FL IK".start();
	$"BR IK".start();
	$"BL IK".start();
	
	legs.append(BossLeg.new($"FR Cast",$"FR target"))
	legs.append(BossLeg.new($"FL Cast",$"FL target"))
	legs.append(BossLeg.new($"BR Cast",$"BR target"))
	legs.append(BossLeg.new($"BL Cast",$"BL target"))
	
	pass # Replace with function body.



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	#operateLegs($"FR Cast",$"FR target")
	#operateLegs($"FL Cast",$"FL target")
	#operateLegs($"BR Cast",$"BR target")
	#operateLegs($"BL Cast",$"BL target")
	
	for leg in legs:
		leg.operate(get_tree())
	
	pass





