using Godot;
using System;
using System.Collections.Generic;

public partial class boss : CharacterBody3D
{

	public static Dictionary<long,Survivor> survivors = new Dictionary<long,Survivor>();

	const float SPEED = 5.0f;
	const float ACCEL = 6.0f;
	const float DEACCEL = 8.0f;
	const float JUMP_VELOCITY = 4.5f;

	const float GRAVITY = 10f;

	bool freeMouse = true;

	public override void _EnterTree(){
		SetMultiplayerAuthority(int.Parse(Name));
	}

	
	
	
	Camera3D camera;
	BossMonsterModel bossMesh;
	Node3D neck;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("neck/head/Camera");
        bossMesh = GetNode<BossMonsterModel>("%bossMonsterRigged");
        neck = GetNode<Node3D>("neck");

        if (IsMultiplayerAuthority()){
			camera.MakeCurrent();
			GetNode<Node3D>("%headMesh").Visible = false;
		}

		HandleMouseModeInputs();

	}

	public void HandleMouseModeInputs(){
		//toggle mouse being locked
		if (Input.IsActionJustPressed("escape")){
			freeMouse = !freeMouse;
		}
		else if (freeMouse && Input.IsActionJustPressed("leftClick")){
			freeMouse = false;
		}
		Input.MouseMode = freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured; 

	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority())return;

		HandleMouseModeInputs();
		Move(delta);

		float targetRotation = (float)(new Vector2(Velocity.Z, Velocity.X).Angle() + Mathf.DegToRad(-90.0));

        float newRotation = (float)Mathf.LerpAngle(bossMesh.Rotation.Y, targetRotation, 3 * delta);

        bossMesh.Rotation = new Vector3(0, newRotation , 0);
		GD.Print(bossMesh.RotationDegrees);

		if (Input.IsActionJustPressed("leftClick"))
		{
			bossMesh.animationPlayer.Play("bite");

        }else if (!bossMesh.animationPlayer.IsPlaying()){
            bossMesh.animationPlayer.Play("idle");
        }

    }


	public void Move(double delta){
		if (! IsOnFloor()){
			Velocity = Velocity + (Vector3.Down * (float)(GRAVITY * delta));
		}

		//Handle Jump
		if (Input.IsActionJustPressed("jump") && IsOnFloor()){
			Velocity = new Vector3(Velocity.X,JUMP_VELOCITY,Velocity.Z);
		}
		
		// Get the input direction and handle the movement/deceleration.
		Vector2 input_dir = Input.GetVector("left", "right", "forward", "backward");
		//we set the forward direction to where the body is facing.
		Vector3 direction = (neck.GlobalTransform.Basis * new Vector3(input_dir.X, 0, input_dir.Y)).Normalized() * SPEED;
		if (direction != Vector3.Zero){
			// Y is up and down, so we don't want to change it.
			
			Velocity = new Vector3(
							(float)Mathf.Lerp(Velocity.X,direction.X,ACCEL * delta),
							Velocity.Y,
							(float)Mathf.Lerp(Velocity.Z,direction.Z,ACCEL * delta)
							);
		}
		else {
			Velocity = new Vector3(
							(float)Mathf.Lerp(Velocity.X,direction.X,DEACCEL * delta),
							Velocity.Y,
							(float)Mathf.Lerp(Velocity.Z,direction.Z,DEACCEL * delta)
							);
		}
		
		MoveAndSlide();
	}

	const double SENSITIVITY = 0.0015f;


	public override void _Input(InputEvent @event)
	{
		//don't move camera if mouse is captive or we're not the authority
		if (!IsMultiplayerAuthority() || freeMouse){return;}
		
		if (@event is InputEventMouseMotion){
			
			InputEventMouseMotion newEvent = @event as InputEventMouseMotion;

			Node3D head = GetNode<Node3D>("neck/head");
			

            head.RotateX((float)(-newEvent.Relative.Y * SENSITIVITY));


            neck.RotateY((float)(-newEvent.Relative.X * SENSITIVITY));
			
			//Clamp head lookup and down
			head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-90),Mathf.DegToRad(90)), head.Rotation.Y, head.Rotation.Z); 

		}
	}
		


}




