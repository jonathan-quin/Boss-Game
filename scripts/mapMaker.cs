using Godot;
using System;
using System.Collections.Generic;

public partial class mapMaker : Node3D
{
	// Called when the node enters the scene tree for the first time.
	
	public static mapMaker instance;

	BreadthFirstBoard board;
	public List<Node3D> islands = new List<Node3D>();


	[Export] private PackedScene Island;

	public override void _Ready()
	{
		instance = this;
		board = new BreadthFirstBoard();
		board.fillBoardEmpty();
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

	public void generateOnAllClients(int seed){
		Rpc("generate",seed);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void generate(int seed){
		foreach(Node3D node in islands){
			node.QueueFree();
		}
		islands.Clear();

		Room.rand = new Random(seed);
		board.MakeNewBoard(0.8,0.05);
		
		for(int i = 0; i < board.map.Count; i++)
		{
			GenerateIsland(new Vector3(board.map[i].position.X * 2, 0, board.map[i].position.Y * 2), board.map[i].doors);
		}
	}


}
