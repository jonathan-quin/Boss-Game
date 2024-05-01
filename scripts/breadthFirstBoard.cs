using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class BreadthFirstBoard
{   
    public List<Room> map;

    public double newConnectionChance = 0.8;
        

    enum RoomDirection
  {
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3
  }


    public BreadthFirstBoard(){   
        map = new List<Room>();
    }

    /// <summary>
    /// Fills map with "0000" value (empty rooms).
    /// </summary>
    public void fillBoardEmpty(){
        map.Add(new Room(Room.ALL_OPEN, Vector2.Zero));
    }

    public void MakeNewBoard(double _newConnectionChance,double chanceDecrease)
    {
        newConnectionChance = _newConnectionChance;

        map = new List<Room>();
        fillBoardEmpty();

        while (step(chanceDecrease)) {}

        GD.Print(this);

    }

    public bool step(double chanceDecrease)
    {

        List<Vector2> looseEnds = GetLooseEnds();

        if (!looseEnds.Any())
        {
            GD.Print("No loose ends.");
            return false;
        }
             
        foreach (Vector2 pos in looseEnds)
        {
            Room newRoom = Room.GenerateRandomValidRoom(map, pos, newConnectionChance);

            if (newRoom != null)
            {
                map.Add(newRoom);
            } else
            {
                GD.Print("Couldn't place an illegal room ",pos);
            }
            
            //GD.Print(this);
        }


        newConnectionChance -= chanceDecrease;

        return true;
    }


    public List<Vector2> GetLooseEnds()
    {

        List<Vector2> positions = new List<Vector2>();

        foreach (Room room in map)
        {
            foreach (Vector2 pos in room.GetConnectedNeighborPositions())
            {
                if (IsPositionEmpty(pos) && !positions.Contains(pos) && !map.Where(r => r.position == pos).Any() ) positions.Add(pos);
            }
        }

        GD.Print("number of loose ends: ", positions.Count);

        return positions;

    }

    public bool IsPositionEmpty(Vector2 pos)
    {
        return !map.Where(r => r.position == pos).Any();
    }

    public override string ToString()
    {


        Vector2 dimensions = GetRightmostLowestPosition(map) - GetLeftmostHighestPosition(map);

        GD.Print("dimensions: ", dimensions);

        Room[,] rooms = new Room[Math.Abs((int)dimensions.X + 1), Math.Abs((int)dimensions.Y) + 1];

        foreach (Room room in map)
        {
            int x = -(int)(room.position.X - GetRightmostLowestPosition(map).X);
            int y = -(int)(room.position.Y - GetRightmostLowestPosition(map).Y);

            
            

            try
            {
                rooms[x, y] = room;
            }
            catch {
                GD.Print("Pos in array: ", x, " ", y, " did not work");
            }
           
        }

        // string temp = string.Empty;


        string temp = "";
        for (int i = 0; i < rooms.GetLength(1); i++)
        {
            for (int j = 0; j < rooms.GetLength(0); j++)
            {
                Room room = rooms[j, i];

                if (room != null && room.position == Vector2.Zero) { temp += "X"; }
                else
                temp += room != null ? room.ToString() : " ";
            }
            temp += "\n";
        }



        return temp;
    }

    public static Vector2 GetLeftmostHighestPosition(List<Room> map)
    {
        Vector2 leftmostHighestPosition = Vector2.Zero;

        foreach (Room room in map)
        {
            // If leftmostHighestPosition is not initialized or the current room's position is more leftmost or highest
            if (leftmostHighestPosition == Vector2.Zero || room.position.X < leftmostHighestPosition.X || room.position.Y < leftmostHighestPosition.Y)
            {
                leftmostHighestPosition = new Vector2(Mathf.Min(room.position.X, leftmostHighestPosition.X), Mathf.Min(room.position.Y, leftmostHighestPosition.Y));
            }
        }

        return leftmostHighestPosition;
    }

    public static Vector2 GetRightmostLowestPosition(List<Room> map)
    {
        Vector2 rightmostLowestPosition = Vector2.Zero;

        foreach (Room room in map)
        {
            // If rightmostLowestPosition is not initialized or the current room's position is more rightmost or lowest
            if (rightmostLowestPosition == Vector2.Zero || room.position.X > rightmostLowestPosition.X || room.position.Y > rightmostLowestPosition.Y)
            {
                rightmostLowestPosition = new Vector2(Mathf.Max(room.position.X, rightmostLowestPosition.X), Mathf.Max(room.position.Y, rightmostLowestPosition.Y));
            }
        }

        return rightmostLowestPosition;
    }

}