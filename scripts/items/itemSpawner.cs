using Godot;
using System;

public partial class itemSpawner : Node3D
{

	//[Export] public String pathToItem;
	[Export] private PackedScene _itemToSpawn;

	public override void _EnterTree()
	{
		SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);
	}

	bool placedItem = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Process(double delta)
    {
        if (!placedItem && IsMultiplayerAuthority()){
			placedItem = true;

			baseItem newItem = (baseItem)_itemToSpawn.Instantiate();
			Globals.objectHolder.AddChild(newItem,true);
			newItem.GlobalTransform = GlobalTransform;
		}
    }

}
