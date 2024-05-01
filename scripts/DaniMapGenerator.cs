using Godot;
using System;

public partial class DaniMapGenerator : Node2D
{
	BreadthFirstBoard board;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Press space to continue!");
		board = new BreadthFirstBoard();
		board.fillBoardEmpty();
		GD.Print(board);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("generate")){
			board.MakeNewBoard(0.8,0.05);
		}
	}

}
