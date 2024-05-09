using Godot;
using System;
using System.Linq;

public partial class itemSpawner : Node3D, GameStartInterface
{

	[Export]
	public String[] pathToItem;

    [Export]
    public double chanceToCreateItemOutOfOne;

	public override void _EnterTree()
	{
		SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);
	}

    public override void _Ready()
    {
        base._Ready();

		GetNode<Node3D>("Sprite3D").Visible = false;

    }

    /// <summary>
    /// This function from gamestartinterface is called by the server at the start of the game.
    /// It is only run on the server.
    /// </summary>
    public void start()
    {
        if (IsMultiplayerAuthority() && Globals.objectHolder != null){

			//GD.Print(pathToItem);

            Random random = new Random();

            if (random.NextDouble() > chanceToCreateItemOutOfOne) return;

            // Shuffle the array using OrderBy with a random number
            string[] shuffledArray = pathToItem.OrderBy(item => random.Next()).ToArray();

            // Take the first item from the shuffled array
            string firstItem = shuffledArray.FirstOrDefault();

            baseItem newItem = GD.Load<PackedScene>(firstItem).Instantiate() as baseItem;
            Globals.objectHolder.AddChild(newItem,true);
			newItem.GlobalTransform = GlobalTransform;

			
		}
    }
}
