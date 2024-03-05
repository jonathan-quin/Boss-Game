using Godot;
using System;

public partial class CustomMultiplayerSpawner : MultiplayerSpawner
{
    public override void _EnterTree()
    {
        
		Callable newSpawnFunction = Callable.From(() => new Node());

		

		SpawnFunction = newSpawnFunction;
    }
	
	delegate Node myDelegate(string str);

}
