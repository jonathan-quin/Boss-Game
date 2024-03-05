using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] private PackedScene _gameScene;
	private SpinBox _portInput;
	private LineEdit _address;
	private Label _message;
	private int _port = 8910;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_portInput = GetNode<SpinBox>("%port");
		_address = GetNode<LineEdit>("%address");
		_message = GetNode<Label>("%Message");
		
		Input.MouseMode = Input.MouseModeEnum.Visible;

		_portInput.Value = _port;
		
		// Try and guess our own IP
		switch (OS.GetName())
		{
			case "Windows":
				_address.Text = IP.ResolveHostname(OS.GetEnvironment("COMPUTERNAME"), IP.Type.Ipv4);
				break;
			case "macOS":
				_address.Text = IP.ResolveHostname(OS.GetEnvironment("HOSTNAME"), IP.Type.Ipv4);
				break;
		}

		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += FailedToConnect;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_port = (int)_portInput.Value;
	}

	private void _onJoinButtonPressed()
	{
		ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
		
		Error error = peer.CreateClient(_address.Text, _port);
		Multiplayer.MultiplayerPeer = peer;

		if (error != Error.Ok)
		{
			GD.Print("client error " + error);
		}

		_message.Text = "Awaiting connection on port " + _port + " at " + _address.Text;
		
		GD.Print("tried to join");
	}
	
	private void ConnectedToServer()
	{
		GD.Print("connected as client");
		GetTree().ChangeSceneToPacked(_gameScene);
	}

	private void FailedToConnect()
	{
		GD.Print("server does not exist");
	}
	
	private void _onQuitButtonPressed()
	{
		GetTree().Quit();
	}
	
	private void _onHostButtonPressed()
	{
		ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
		
		Error error = peer.CreateServer(_port, 16);

		if (error != Error.Ok)
		{
			GD.Print("host error " + error);
		}

		Multiplayer.MultiplayerPeer = peer;

		GetTree().ChangeSceneToPacked(_gameScene);
	}
}
