using Godot;
using System;

public partial class Knife : baseItem
{
	public override void Use(){

		GD.Print("slash");

		GetNode<AnimationPlayer>("AnimationPlayer").Play("slash");

	}

}
