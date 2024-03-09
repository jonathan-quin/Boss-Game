using Godot;
using System;

public partial class Spectator : CharacterBody3D
{

	const float SPEED = 5.0f;
	const float ACCEL = 6.0f;
	const float DEACCEL = 8.0f;

	//this is set before enter tree is called.
	public int targetAuthority = -1;
	public override void _EnterTree(){
		SetMultiplayerAuthority(targetAuthority);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority() || !Globals.freeMouse){
			return;
		}

		Move(delta);


	}


	public void Move(double delta){
		
		// Get the input direction and handle the movement/deceleration.
		Vector2 input_dir = Input.GetVector("left", "right", "forward", "backward");
		//we set the forward direction to where the body is facing.
		Vector3 direction = (GetNode<Node3D>("Camera3D").GlobalTransform.Basis * new Vector3(input_dir.X, Input.GetAxis("jump","crouch"), input_dir.Y)).Normalized() * SPEED;
		if (direction != Vector3.Zero){
			// Y is up and down, so we don't want to change it.
			
			Velocity = new Vector3(
							(float)Mathf.Lerp(Velocity.X,direction.X,ACCEL * delta),
							(float)Mathf.Lerp(Velocity.Y,direction.Y,ACCEL * delta),
							(float)Mathf.Lerp(Velocity.Z,direction.Z,ACCEL * delta)
							);
		}
		else {
			Velocity = new Vector3(
							(float)Mathf.Lerp(Velocity.X,direction.X,DEACCEL * delta),
							(float)Mathf.Lerp(Velocity.Y,direction.Y,ACCEL * delta),
							(float)Mathf.Lerp(Velocity.Z,direction.Z,DEACCEL * delta)
							);
		}
		
		MoveAndSlide();
	}

	const double SENSITIVITY = 0.0015f;

    public override void _Input(InputEvent @event)
	{
		//don't move camera if mouse is captive or we're not the authority
		if (!IsMultiplayerAuthority() || Globals.freeMouse){return;}
		
		if (@event is InputEventMouseMotion){
			
			InputEventMouseMotion newEvent = @event as InputEventMouseMotion;

			Node3D head = GetNode<Node3D>("Camera3D");

			head.RotateX((float)(-newEvent.Relative.Y * SENSITIVITY));


			RotateY((float)(-newEvent.Relative.X * SENSITIVITY));
			
			//Clamp head lookup and down
			head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-90),Mathf.DegToRad(90)), head.Rotation.Y, head.Rotation.Z); 

		}
	}

}
