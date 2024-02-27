using Godot;
using System;

public partial class ItemHolder : HeldItem
{
	
    

	HeldItem selectedItem;

    //includes the hand
    const int MAX_ITEMS = 3 + 1;

    public override void _Ready()
    {
        selectedItem = GetChild(0) as HeldItem;

    }

    public bool TakeItem(string itemPath)
    {
        if (GetChildCount() >= MAX_ITEMS)
        {
            return false;
        }

		HeldItem item = GD.Load<PackedScene>(itemPath).Instantiate() as HeldItem;
		item.SetMultiplayerAuthority(GetMultiplayerAuthority());
		AddChild(item);

        selectedItem = item;

        return true;
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
            //GD.Print(selectedItem);
            selectedItem.Use();
        }

        if (Input.IsActionJustPressed("drop") && selectedItem.floorForm != null)
        {
            FloorItem newItem = selectedItem.floorForm.Instantiate() as FloorItem;

            Globals.objectHolder.AddChild(newItem);

            newItem.GlobalPosition = GlobalPosition + GlobalTransform.Basis.Z * -0.5f;
            newItem.GlobalRotation = GlobalRotation;

            float throwForce = 5f;
            newItem.ApplyImpulse(GlobalTransform.Basis.Z * -throwForce);

            HeldItem itemToDelete = selectedItem;

            shiftSelection(1);
            itemToDelete.QueueFree();

        }

    }

    public void shiftSelection(int amount)
    {

        if (amount == 0) return;

        //GD.Print("shift");

        int currentSelection = GetChildren().IndexOf(selectedItem);

        int newSelection = currentSelection + amount;
        int childCount = GetChildCount();

        if (newSelection >= childCount) {
            newSelection -= childCount;
        }
        if (newSelection < 0)
        {
            newSelection = childCount - 1;
        }

        selectedItem = GetChild(newSelection) as HeldItem;
    }



}
