using Godot;
using System;

public partial class deleteOnEnterTree : Sprite3D
{

    /// <summary>
    /// Helper class for things we only want to show in the editor.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        QueueFree();
    }
}
