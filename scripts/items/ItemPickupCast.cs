using Godot;
using System;

public partial class ItemPickupCast : RayCast3D
{

	/// <summary>
	/// This object is owned by the player. ALl it does it tell items to reparent themselves to the player.
	/// </summary>

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	baseItem lastItemLookedAt = null;

	
	public override void _Process(double delta)
	{

		if (lastItemLookedAt != null)
		{
            lastItemLookedAt.shouldShimmer = false;
        }
        
        
		if (IsColliding()){
			
            lastItemLookedAt = GetCollider() as baseItem;

			if (lastItemLookedAt != null)
			{
                lastItemLookedAt.shouldShimmer = true;
            }
            


            if (Input.IsActionJustPressed("leftClick"))
			{

				if (!lastItemLookedAt.heldByPlayer) lastItemLookedAt.giveToSurvivor();

			}

		}

	}
}
