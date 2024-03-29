using Godot;
using System;

public partial class itemSpawner : Node3D, GameStartInterface
{

	[Export]
	public String pathToItem;

	public override void _EnterTree()
	{
		SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);
	}

    public override void _Ready()
    {
        base._Ready();

		GetNode<Node3D>("Sprite3D").Visible = false;

    }

    public void start()
    {
        if (IsMultiplayerAuthority() && Globals.objectHolder != null){

			//GD.Print(pathToItem);

            baseItem newItem = GD.Load<PackedScene>(pathToItem).Instantiate() as baseItem;
            Globals.objectHolder.AddChild(newItem,true);
			newItem.GlobalTransform = GlobalTransform;

			
		}
    }
}
