using Godot;
using System;

public partial class damageArea : Node3D
{

    /// <summary>
    /// This node should only ever exist on the server. It is not synced.
    /// </summary>
    /// 



	public Area3D area3D;
	public TakeDamageInterface.TypeOfEntity targetEntity = TakeDamageInterface.TypeOfEntity.PLAYER;

	public double damage = 20;

    public override void _Ready()
    {
        area3D = GetNode<Area3D>("Area3D");

    }

    public override void _Process(double delta)
    {
        base._Process(delta);

		foreach (Node3D body in area3D.GetOverlappingAreas()){
			TakeDamageInterface damageTaker = body.GetParent() as TakeDamageInterface;
			
			if (damageTaker != null && targetEntity == damageTaker.typeOfEntity){
				damageTaker.TakeDamage(damage);
			}

		}

    }

}
