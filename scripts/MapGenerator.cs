using Godot;
using System;

public partial class MapGenerator : Node2D
{
	Board board;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Press space to continue!");
		board = new Board();
		board.fillBoardEmpty();
		board.placeFirstRoom();
		GD.Print(board);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("continue")){
			board.addNextPartOfMap();
			GD.Print(board.getBoardString());
		}
	}

}
