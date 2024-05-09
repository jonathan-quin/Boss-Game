using Godot;
using System;

public partial class damageArea : Node3D
{

	/// <summary>
	/// This node should only ever exist on the server. It is not synced.
	/// </summary>
	/// 



	public Area3D area3D;
	public int targetEntity = TakeDamageInterface.TypeOfEntity.SURVIVOR.GetHashCode();

	public double damage = 20;

	//number of frames the area will exist. Should be more than one so that the area has time to register, move than 2 for safety.
	public int lifeSpan = 30;

	public override void _Ready()
	{
		area3D = GetNode<Area3D>("Area3D");

	}

	/// <summary>
	/// searches for something to damage. This object is intended to exist only on the server.
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(double delta)
	{
		base._Process(delta);

		//Since it's searching for a hitbox instead of an object, it grabs the parent of whatever it collides with.
		//hitboxes should be children of the thing that takes damage.
		foreach (Node3D body in area3D.GetOverlappingAreas()){

			//GD.Print("found someone to hit!");

			TakeDamageInterface damageTaker = body.GetParent() as TakeDamageInterface;
			
			if (damageTaker != null && targetEntity == damageTaker.typeOfEntity){
				damageTaker.TakeDamage(damage);

				//makes sure the area knows it's done
				//yes we could hit multiple targets.
				lifeSpan = -1;
			}

		}

		lifeSpan--;
		if (lifeSpan < 0){
			QueueFree();
		}

	}

}
