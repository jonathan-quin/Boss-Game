using Godot;
using System;

public partial class deleteOnEnterTree : Sprite3D
{
    public override void _EnterTree()
    {
        base._EnterTree();
        QueueFree();
    }
}
