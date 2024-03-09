using Godot;
using System;

public partial interface TakeDamageInterface
{

	public enum TypeOfEntity {SURVIVOR,BOSS}

	public int typeOfEntity {get;set;}

	public double health {get;set;}
	public bool dead {get;set;}
	public abstract void TakeDamage(double amount);
	public abstract void Die();

}
