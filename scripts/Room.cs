using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Room
{
    public static int[] ALL_OPEN = { 1, 1, 1, 1 };
    public static int[] ALL_CLOSED = { 0, 0, 0, 0 };

    public static Dictionary<string, string> roomConnectionTypes = new Dictionary<string, string>();

    //1 means open, 0 means closed
    //UP RIGHT DOWN LEFT
    public int[] doors = {0,0,0,0};

    public Vector2 position = Vector2.Zero;

    public Room(int[] _doors, Vector2 position)
    {
        this.doors = CopyArray(_doors);
        this.position = position;

        if (!roomConnectionTypes.Keys.Any())
        {
            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 0, 0, 0 }), " ");
            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 0, 0, 0 }), "▲");
            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 1, 0, 0 }), "►");
            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 0, 1, 0 }), "▼");
            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 0, 0, 1 }), "◄");

            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 1, 1, 0 }), "╔");
            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 1, 0, 0 }), "╚");
            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 0, 1, 0 }), "║");
            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 1, 0, 1 }), "═");
            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 0, 1, 1 }), "╗");
            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 0, 0, 1 }), "╝");

            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 1, 1, 0 }), "╠");
            roomConnectionTypes.Add(ArrayToString(new int[] { 0, 1, 1, 1 }), "╦");
            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 1, 0, 1 }), "╩");
            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 0, 1, 1 }), "╣");

            roomConnectionTypes.Add(ArrayToString(new int[] { 1, 1, 1, 1 }), "╬");
        }

    }

    private static int[] CopyArray(int[] array)
    {
        int[] copy = new int[array.Length];
        Array.Copy(array, copy, array.Length);
        return copy;
    }

    private string ArrayToString(int[] array)
    {
        return string.Join("", array);
    }

    //returns all the positions that the doors lead to, whether there are rooms there or not. I DON'T KNOW WHY THOSE ARE ALL THE OPPISITE DIRECTION
    public List<Vector2> GetConnectedNeighborPositions()
    {
        List<Vector2> result = new List<Vector2>();

        if (doors[0] == 1) result.Add(position + Vector2.Down);
        if (doors[1] == 1) result.Add(position + Vector2.Left);
        if (doors[2] == 1) result.Add(position + Vector2.Up);
        if (doors[3] == 1) result.Add(position + Vector2.Right);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="position"></param>
    /// <param name="chanceToAddExtraDoors"> 1 means 100% likely, 0 means never</param>
    /// <returns></returns>
    public static Room GenerateRandomValidRoom(List<Room> map, Vector2 position, double chanceToAddExtraDoors)
    {
        Room newRoom = new Room(ALL_CLOSED, position);

        Room[] neighbors = map.Where(r => r.position == position + Vector2.Right 
                                    || r.position == position + Vector2.Left 
                                    || r.position == position + Vector2.Down 
                                    || r.position == position + Vector2.Up).ToArray();

        bool nextToSomething = false;

        //force nessasary connections open
        foreach (Room r in neighbors)
        {
            Vector2 opisiteDirectionToRoom = r.position - newRoom.position;

            if (r.connectionOpen(opisiteDirectionToRoom))
            {
                newRoom.doors[GetDirectionIndex(-opisiteDirectionToRoom)] = 1;
                nextToSomething = true;
            }
        }

        if (!nextToSomething) return null;

        //GD.Print(newRoom, " This is after step 1");

        //add extra connections
        Random rand = new Random(/*Globals.Seed*/);

        for (int i = 0; i <newRoom.doors.Length; i++)
        {
            if (newRoom.doors[i] == 0 && rand.NextDouble() < chanceToAddExtraDoors)
            {
                newRoom.doors[i] = 1;
            }
        }

        //GD.Print(newRoom, " This is after step 2");

        //force illegal connections closed
        foreach (Room r in neighbors)
        {
            Vector2 opisiteDirectionToRoom = r.position - newRoom.position;

            if (!r.connectionOpen(opisiteDirectionToRoom))
            {
                newRoom.doors[GetDirectionIndex(-opisiteDirectionToRoom)] = 0;
            }
        }

        //GD.Print(newRoom, " This is after step 3");

        return newRoom;
    }

    public bool connectionOpen(Vector2 direction)
    {
        return doors[GetDirectionIndex(direction)] == 1;
    }

    // Helper method to get the direction vector based on the index
    private static Vector2 GetDirectionVector(int index)
    {
        switch (index)
        {
            case 0:
                return Vector2.Up;
            case 1:
                return Vector2.Right;
            case 2:
                return Vector2.Down;
            case 3:
                return Vector2.Left;
            default:
                return Vector2.Zero;
        }
    }

    private static int GetDirectionIndex(Vector2 direction)
    {
        if (direction == Vector2.Up)
            return 0;
        else if (direction == Vector2.Right)
            return 1;
        else if (direction == Vector2.Down)
            return 2;
        else if (direction == Vector2.Left)
            return 3;
        else
            return -1; // Invalid direction
    }

    public override string ToString()
    {

        //return position.ToString();

        return roomConnectionTypes[ArrayToString(doors)];

    }



}