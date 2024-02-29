using Godot;
using System;

[Tool]
public partial class ItemSpawnerDisplay : Sprite3D
{
	
	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			Visible = false;
		}
		else
		{
			Visible = true;
		}
	}
	
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			LookAt(EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D().Position);
		}
	}
}
