using Godot;
using System;

public partial class Globals : Node
{   
    
    public static Node3D objectHolder;

    public static CustomMultiplayerSpawner multiplayerSpawner { get; internal set; }

    public static bool freeMouse = true;

    public static string nameTagText;
    public static bool isBoss;

    public static int Seed;

    //only correct on server
    public static bool gameInProgress = false;
}
