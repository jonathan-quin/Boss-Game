using Godot;
using System;

public partial class ItemHolder : HeldItem
{
	
	HeldItem selectedItem;

    public override void _Ready()
    {
        selectedItem = GetChild(0) as HeldItem;

    }

    public void TakeItem(string itemPath)
    {
		HeldItem item = GD.Load<PackedScene>(itemPath).Instantiate() as HeldItem;
		item.SetMultiplayerAuthority(GetMultiplayerAuthority());
		AddChild(item);
	}

    public override void _PhysicsProcess(double delta)
    {
        if (!IsMultiplayerAuthority()) return;

        int shift = 0; 
        if (Input.IsActionJustPressed("itemSelectForward")){
            shift += 1;
        }
        if (Input.IsActionJustPressed("itemSelectBackward"))
        {
            shift -= 1;
        }
        shiftSelection(shift);

        foreach (Node3D child in GetChildren())
        {
            child.Visible = false;
        }

        selectedItem.Visible = true;

        if (Input.IsActionJustPressed("leftClick"))
        {
            GD.Print(selectedItem);
            selectedItem.Use();
        }

        if (Input.IsActionJustPressed("drop"))
        {

        }

    }

    public void shiftSelection(int amount)
    {

        if (amount == 0) return;

        GD.Print("shift");

        int currentSelection = GetChildren().IndexOf(selectedItem);

        int newSelection = currentSelection + amount;

        if (newSelection > GetChildCount() - 1) {
            newSelection = 0;
        }
        if (newSelection < GetChildCount())
        {
            newSelection = GetChildCount() - 1;
        }

        selectedItem = GetChild(newSelection) as HeldItem;
    }



}
