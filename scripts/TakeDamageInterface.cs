using Godot;
using System;

public partial interface TakeDamageInterface
{

	public double health {get;set;}
	public abstract void TakeDamage(double amount);
}
