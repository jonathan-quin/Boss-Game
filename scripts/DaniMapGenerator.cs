using Godot;
using System;

public partial class DaniMapGenerator : Node2D
{
	BreadthFirstBoard board;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Press space to continue!");
		
		board = new BreadthFirstBoard(GD.RandRange(-2147483647, 2147483647));
		board.fillBoardEmpty();
		GD.Print(board);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("generate")){
			Globals.Seed = 40;
			Room.rand = new Random(Globals.Seed);
			board.MakeNewBoard(0.8,0.05);
		}
	}

}
