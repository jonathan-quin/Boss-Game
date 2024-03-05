using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public partial class MainMenu : Control
{
    [Export] private String _gameScene;
    private SpinBox _portInput;
    private LineEdit _address;
    private Label _message;
    private int _port = 8910;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
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
                _address.Text = GetLocalIPAddress();
                break;
        }

        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += FailedToConnect;
        GetNode<Button>("%host button").Pressed += _onHostButtonPressed;
        GetNode<Button>("%join button").Pressed += _onJoinButtonPressed;
        GetNode<Button>("%quit button").Pressed += _onQuitButtonPressed;

        if (Constants.DEBUG_MODE){
            
            debugStart();

        }

    }

    public async Task debugStart(){
        
        await ToSignal(GetTree().CreateTimer(1), "timeout");

        _onJoinButtonPressed();
    }

    public static string GetLocalIPAddress()
    {
        //other code didn't work, gave up.
        return "10.135.16.136";

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
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
        GetTree().ChangeSceneToFile(_gameScene);
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

        GetTree().ChangeSceneToFile(_gameScene);
    }
}