using Godot;
using System;

public partial class handItem : HeldItem
{
    public override void Use()
    {
        GD.Print("punch");
    }
}
