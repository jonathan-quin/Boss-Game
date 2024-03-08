using Godot;
using System;

public partial interface TakeDamageInterface
{

	public enum TypeOfEntity {PLAYER,BOSS}

	public TypeOfEntity typeOfEntity {get;set;}

	public double health {get;set;}
	public bool dead {get;set;}
	public abstract void TakeDamage(double amount);
	public abstract void Die();

}
