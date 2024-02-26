using Godot;
using System;

public partial class ItemHolder : Node3D
{
	
	HeldItem selectedItem;

	public void TakeItem(string itemPath)
    {

		HeldItem item = GD.Load<PackedScene>(itemPath).Instantiate() as HeldItem;
		item.SetMultiplayerAuthority(GetMultiplayerAuthority());
		AddChild(item);
	}

	public void _PhysicsProcess(){
		if (Input.IsActionJustPressed("leftClick")){
			selectedItem.Use();
		}

		if (Input.IsActionJustPressed("drop")){
			
		}

	}

}
