using Godot;
using System;
using System.Collections.Generic;

public partial class ItemHolder : Node3D
{
	
    

	baseItem selectedItem = null;
    List<baseItem> inventory = new List<baseItem>();

    public static ItemHolder localItemHolder;

    
    const int MAX_ITEMS = 3 + 1;

    public override void _Ready()
    {
        localItemHolder = this;
    }

    /// <summary>
    /// only called on the client
    /// </summary>
    /// <param name="itemPath"></param>
    /// <returns></returns>
    public bool TakeItem(baseItem item)
    {
        if (GetChildCount() >= MAX_ITEMS)
        {
            return false;
        }

        //GD.Print("we get here");

		inventory.Add(item);

        selectedItem = item;

        return true;
	}

    public override void _PhysicsProcess(double delta)
    {
        if (!IsMultiplayerAuthority()) return;


        handleItem();

        

    }

    public void handleItem(){
        
        

        if (inventory.Count <= 0){
            selectedItem = null;
            return;
        }

        //GD.Print("inventory is not empty");


        if (selectedItem == null){
            selectedItem = inventory[0];
        }

        int shift = 0; 
        if (Input.IsActionJustPressed("itemSelectForward")){
            shift += 1;
        }
        if (Input.IsActionJustPressed("itemSelectBackward"))
        {
            shift -= 1;
        }
        shiftSelection(shift);

        

        foreach (baseItem item in inventory)
        {
            item.Visible = false;
            item.GlobalTransform = GlobalTransform;
            //GD.Print("yes we are syncing the position");
        }

        selectedItem.Visible = true;

        if (Input.IsActionJustPressed("leftClick"))
        {
            //GD.Print(selectedItem);
            selectedItem.Use();
        }

        if (Input.IsActionJustPressed("drop") && selectedItem.pathToSelf != null)
        {

            throwItem();
            

            /*baseItem newItem = GD.Load<PackedScene>(selectedItem.pathToSelf).Instantiate() as baseItem;

            newItem.SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);

            Globals.objectHolder.AddChild(newItem);

            newItem.GlobalPosition = GlobalPosition + GlobalTransform.Basis.Z * -0.5f;
            newItem.GlobalRotation = GlobalRotation;

            float throwForce = 5f;
            newItem.ApplyImpulse(GlobalTransform.Basis.Z * -throwForce);

            baseItem itemToDelete = selectedItem;

            shiftSelection(1);
            itemToDelete.QueueFree();*/

        }
    }



    public void throwItem(){
       
       baseItem itemToThrow = selectedItem;
       
        if (inventory.Count > 1){
            shiftSelection(1);
            inventory.Remove(itemToThrow);
        }else{
            inventory.Remove(itemToThrow);
            selectedItem = null;
        }

        itemToThrow.throwSelf(GlobalTransform);
        
        //GD.Print("removed it ",inventory.Count);
        
    }

    public void shiftSelection(int amount)
    {
        
        if (amount == 0) return;

        //GD.Print("shift");

        int currentSelection = 0;
        
        if (inventory.Count > 0){
          currentSelection = inventory.IndexOf(selectedItem);
        }
         

        int newSelection = currentSelection + amount;
        int totalItems = inventory.Count;

        if (newSelection >= totalItems) {
            newSelection -= totalItems;
        }
        if (newSelection < 0)
        {
            newSelection = totalItems - 1;
        }

        selectedItem = inventory[newSelection];
    }



}
