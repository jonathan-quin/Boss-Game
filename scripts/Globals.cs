using Godot;
using System;

public partial class Globals : Node
{   
    
    public static Node3D objectHolder;

    public static MultiplayerSpawner multiplayerSpawner { get; internal set; }

    public static Vector3 PlayerSpawnPoints;
}
