using Godot;
using System;
using System.Collections.Generic;


public partial class gameStartRequest : Node
{
	public enum GameMode {RANDOM_BOSS,REQUEST_BOSS,REQUEST_ONE_BOSS,HOST_BOSS}

	public List<PlayerConfiguration> playerConfigurations;

	public GameMode gameMode;

    public gameStartRequest(List<PlayerConfiguration> playerConfigurations, GameMode gameMode)
    {
        this.playerConfigurations = playerConfigurations;
        this.gameMode = gameMode;
    }



}
