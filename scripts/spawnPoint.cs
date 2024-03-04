using Godot;
using System;

[Tool]

public partial class spawnPoint : Node3D
{

	public enum SpawnType { SURVIVOR, BOSS };


	[Export]
	public SpawnType spawnType = SpawnType.SURVIVOR;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

        //GD.Print("cry");

        if (!Engine.IsEditorHint()) return;

		//GD.Print("heyo");

		switch (spawnType)
		{
			case SpawnType.SURVIVOR:
                GetNode<Node3D>("playerIcon").Visible = true;
                GetNode<Node3D>("bossIcon").Visible = false;
				break;
            case SpawnType.BOSS:
                GetNode<Node3D>("playerIcon").Visible = false;
                GetNode<Node3D>("bossIcon").Visible = true;
                break;

        }
	}
}
