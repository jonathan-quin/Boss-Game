using Godot;
using System;

public partial class island : Node3D
{

	[Export] private PackedScene Bridge;
	float bridgeDisplacement = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void CreateBridge(int direction, int open){
		Node3D bridge = Bridge.Instantiate() as Node3D;
		
		if(open == 1) bridge.GetNode<BlockingWall>("BlockingWall").Disable();

		switch (direction){
			case 0: // UP
				bridge.Position = new Vector3(0, 0, bridgeDisplacement);
				bridge.RotateY((float) Math.PI /2);
				break;
			case 1: // RIGHT
				bridge.Position = new Vector3(-bridgeDisplacement, 0, 0);
				break;
			case 2: // DOWN
				bridge.Position = new Vector3(0, 0, -bridgeDisplacement);
				bridge.RotateY((float) Math.PI /2);
				break;
			case 3: // LEFT
				bridge.Position = new Vector3(bridgeDisplacement, 0, 0);
				break;

		}

		AddChild(bridge);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
