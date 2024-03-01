extends Node

class_name BossLeg

var stepLength = 3
var stepHeight = 1

var stepDuration = 0.5

var raycast : RayCast3D
var target : Marker3D

var stepping = false

var buddy : BossLeg
var allLegs : Array

var targetPosition

func _init(raycast,target,allLegs):
	self.raycast = raycast
	self.target = target
	self.allLegs = allLegs
	targetPosition = target.global_position
	pass

func setBuddy(buddy):
	
	self.buddy = buddy;
	

func setTargetPosition():
	
	if !is_instance_valid(tween) or !tween.is_running():
		target.global_position = targetPosition
	
	

func operate(tree):
	if raycast.is_colliding() and raycast.get_collision_point().distance_to(target.global_position) > stepLength:
		
		var shouldStep = false
		
		for leg in allLegs:
			if leg.stepping == false and leg != buddy:
				shouldStep = true
				break
		
		buddy.step(tree)
		step(tree)
		
	
	setTargetPosition()
	

var tween : Tween

func step(tree):
	
	if (tween != null and tween.is_valid()):
		tween.kill()
	
	tween = tree.create_tween();
	
	var newLegPos = raycast.get_collision_point()
	var midPos = (target.global_position + newLegPos)/2 + stepHeight * Vector3.UP
	
	tween.tween_property(target,"global_position",midPos,stepDuration/2)
	tween.tween_property(target,"global_position",newLegPos,stepDuration/2)
	
	targetPosition = newLegPos
	
	pass

pass
