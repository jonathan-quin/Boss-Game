using Godot;
using System;

public partial class ItemPickupCast : RayCast3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	FloorItem lastItemLookedAt = null;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (lastItemLookedAt != null)
		{
            lastItemLookedAt.shouldShimmer = false;
        }
        
        
		if (IsColliding()){
			
            lastItemLookedAt = GetCollider() as FloorItem;

			if (lastItemLookedAt != null)
			{
                lastItemLookedAt.shouldShimmer = true;
            }
            


            if (Input.IsActionJustPressed("leftClick"))
			{
                lastItemLookedAt.giveToSurvivor();
			}

		}

	}
}
