using Godot;
using System;
using System.Collections.Generic;

public partial class main : Node3D
{
	// Called when the node enters the scene tree for the first time.


	BreadthFirstBoard board;
	public List<Node3D> islands = new List<Node3D>();


	[Export] private PackedScene Island;

	public override void _Ready()
	{
		GD.Print("Press space to continue!");
		board = new BreadthFirstBoard();
		board.fillBoardEmpty();
		GD.Print(board);
		
	}

	public void GenerateIsland(Vector3 position, int[] doors){
		island island  = Island.Instantiate() as island;
		island.Position = position;

		island.GetNode<MeshInstance3D>("MeshInstance3D").GetSurfaceOverrideMaterial(0).Set("albedo_color", new Color(GD.Randf(), GD.Randf(), GD.Randf()));
		
		for(int i = 0; i < doors.Length; i++){
			island.CreateBridge(i, doors[i]);
		}
		
		islands.Add(island); 
		AddChild(island);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("generate"))
		{
			foreach(Node3D node in islands){
				node.QueueFree();
			}
			islands.Clear();

			if(Input.IsActionJustPressed("generate")){
			board.MakeNewBoard(0.8,0.05);
		}
			for(int i = 0; i < board.map.Count; i++)
			{
				GenerateIsland(new Vector3(board.map[i].position.X * 2, 0, board.map[i].position.Y * 2), board.map[i].doors);
			}
			
		}
    }

}
