using Godot;
using System;
using System.Collections.Generic;

public partial class lobbyInterface : Control
{

    /// <summary>
    /// This class handles connecting to the server and telling the lobby to start the game.
    /// It has a list of controll nodes that represent the players in the server. 
    /// Each control node can only be changed by the player who owns it.
    /// </summary>

    [Export]
    public PackedScene lobbyPlayerScene;

    public Container playerContainer;
    public Button startGameButton;
    public Button forceEndGameButton;

    public Lobby lobby;

    public PauseMenu pauseMenu;

    public static lobbyInterface instance;

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _Ready()
    {

        lobby = GetParent() as Lobby;
        playerContainer = GetNode<Container>("%playerVboxContainer");

        pauseMenu = GetNode<PauseMenu>("%pauseMenu");
        pauseMenu.lobbyInterface = this;

        startGameButton = GetNode<Button>("%startGameButton");
        startGameButton.Pressed += StartGame;

        forceEndGameButton = GetNode<Button>("%forceEndGame");
        forceEndGameButton.Pressed += lobby.endGame;


        //all players have these signals connected for debugging
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += FailedToConnect;

        if (Multiplayer.IsServer())
        {
            //the server is set to create new players immediately when they join, but this behavior will be changed later to not interupt games.
            Multiplayer.PeerConnected += CreatePlayer;
            Multiplayer.PeerDisconnected += DeletePlayer;

            CreatePlayer();
        }
        else
        {
            Multiplayer.ServerDisconnected += ServerDisconnected;
            startGameButton.Disabled = true;
            forceEndGameButton.Disabled = true;
        }
    }

    public void ConnectedToServer()
    {
        GD.Print("Connected! " + Multiplayer.GetUniqueId());
    }
    public void FailedToConnect()
    {
        GD.Print("Failed to connect. " + Multiplayer.GetUniqueId());
    }

    public void ServerDisconnected()
    {

        pauseMenu.goToMenu();

    }

    public override void _PhysicsProcess(double delta)
    {
        HandlePauseInputs();

        if (Multiplayer.IsServer())
        {
            startGameButton.Disabled = Globals.gameInProgress;
        }

    }

    public static List<lobbyPlayer> lobbyPlayers = new List<lobbyPlayer>();


    /// <summary>
    /// Tells the lobby to start the game based off of the information the players have given from their lobbyPlayer nodes.
    /// </summary>
    public void StartGame()
    {



        List<PlayerConfiguration> configList = new List<PlayerConfiguration>();

        foreach (lobbyPlayer player in lobbyPlayers)
        {
            configList.Add(player.getPlayerConfig());
        }

        gameStartRequest startRequest = new gameStartRequest(configList, gameStartRequest.GameMode.REQUEST_BOSS);


        lobby.StartGame(startRequest);

    }

    /**
	Used only by server
	Spawns a player. The MultiplayerSpawner will automatically replicate the instance on all clients.
	**/
    public void CreatePlayer(long id = 1)
    {
        //GD.Print("made a player");

        var player = lobbyPlayerScene.Instantiate() as lobbyPlayer;// as Survivor;
        player.Name = id.ToString();
        playerContainer.AddChild(player);

        lobbyPlayers.Add(player);


    }


    /**
	Used only by server
	Called when multiplayer detects a client has left.
	removes the client's avatar and then syncs with all clients through multiplayer spawner.
	**/
    public void DeletePlayer(long id = 1)
    {
        if (id != 1)
        {
            lobbyPlayer player = playerContainer.GetNode<Node>(id.ToString()) as lobbyPlayer;
            lobbyPlayers.Remove(player);
            player.QueueFree();


            lobby.DeletePlayer(id);
        }
    }

    /// <summary>
    /// Used for swapping between the paused state and the captured mouse state.
    /// 
    /// 
    /// </summary>
    public void HandlePauseInputs()
    {



        if (Input.IsActionJustPressed("pause"))
        {
            Globals.freeMouse = !Globals.freeMouse;

            if (Globals.freeMouse)
            {
                Visible = true;
            }
            else
            {
                Visible = false;
            }


        }

        //litterally determining if the game is over by if everything is deleted apparently. Kinda terrible code.
        if (!Globals.freeMouse)
        {
            var gameStartNodes = GetTree().GetNodesInGroup("deleteOnGameEnd");
            if (gameStartNodes.Count <= 0)
            {
                open();
            }
        }


        Input.MouseMode = Globals.freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;

    }

    public void resume()
    {
        Globals.freeMouse = false;

        Visible = false;

        Input.MouseMode = Globals.freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }

    public void open()
    {
        Globals.freeMouse = true;

        Visible = true;

        Input.MouseMode = Globals.freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }



}


