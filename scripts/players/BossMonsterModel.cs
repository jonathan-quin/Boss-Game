using Godot;
using System;

public partial class BossMonsterModel : Node3D
{

	//This class only exists the make the boss mesh's animation player accessable.

	public AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		

	}
}
