extends Node

class_name BossLeg

var stepLength = 3

var stepHeight = 1

var stepDuration = 0.3

#force step
var forceStepLength = 6
var forceStepDuration = 0.1

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
	buddy.buddy = self;
	

func setTargetPosition():
	
	if !is_instance_valid(tween) or !tween.is_running():
		target.global_position = targetPosition
	
	

func operate(tree,velocity:Vector3):
	if raycast.is_colliding() and raycast.get_collision_point().distance_to(target.global_position) > stepLength and !stepping:
		
		var shouldStep = true
		
		#print("checking")
		for leg in allLegs:
			#print(leg)
			
			#return true if the leg is stepping and not our buddy.
			if leg.stepping:
				if buddy == null:
					shouldStep = false
				elif leg != buddy:
					shouldStep = false
				#break
		
		if shouldStep:
			if buddy:
				buddy.step(tree,velocity)
			step(tree,velocity)
		#else:
			#print("can't step")
		
	
	if raycast.is_colliding() and raycast.get_collision_point().distance_to(target.global_position) > forceStepLength:
		forceStep(tree,velocity)
	
	setTargetPosition()
	

var tween : Tween

#velocity needs to be in meters per second
func step(tree,velocity):
	
	if (tween != null and tween.is_valid()):
		tween.kill()
	
	tween = tree.create_tween();
	
	var newLegPos = raycast.get_collision_point() + velocity * stepDuration 
	#print(velocity)
	
	var midPos = (target.global_position + newLegPos)/2 + stepHeight * Vector3.UP
	
	stepping = true
	tween.tween_property(target,"global_position",midPos,stepDuration/2)
	tween.tween_property(target,"global_position",newLegPos,stepDuration/2)
	tween.tween_property(self,"stepping",false,0.01)
	
	targetPosition = newLegPos
	
	pass

func forceStep(tree,velocity):
	#print("force stepping")
	if (tween != null and tween.is_valid()):
		tween.kill()
	stepping = false;
	
	tween = tree.create_tween();
	
	var newLegPos = raycast.get_collision_point() + velocity * stepDuration 
	#print(velocity)
	
	var midPos = (target.global_position + newLegPos)/2 + stepHeight * Vector3.UP
	tween.tween_property(target,"global_position",midPos,stepDuration/2)
	tween.tween_property(target,"global_position",newLegPos,stepDuration/2)
	targetPosition = newLegPos
	

pass
