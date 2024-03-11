using Godot;
using System;

public partial class Holder : StaticBody3D
{
	public int ArtifactsContained = 0;
	private Lobby lobby;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Area3D>("%Artifact Detection Field").BodyEntered += _onBodyEnterDetectionField;
		lobby = GetParent<Lobby>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<MeshInstance3D>("%Artifact01").Visible = (ArtifactsContained >= 1);
		GetNode<MeshInstance3D>("%Artifact02").Visible = (ArtifactsContained >= 2);
		GetNode<MeshInstance3D>("%Artifact03").Visible = (ArtifactsContained >= 3);

		if (ArtifactsContained >= 3)
		{
			lobby.endGame();
			ArtifactsContained = 0;
		}
	}

	private void _onBodyEnterDetectionField(Node3D body)
	{
		if (body.IsInGroup("artifact"))
		{
			body.QueueFree();
			Rpc("_handleArtifactInput");
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void _handleArtifactInput()
	{
		ArtifactsContained++;
	}
}
