using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class Board
{   
    public string[,] map = new string[100,20];
    public Random random = new Random();
    public Dictionary<string, string> roomConnectionTypes = new Dictionary<string, string>();
    public ArrayList openEndedRooms = new ArrayList();
    public string debugText = "";
    // Properties below were manually dragged out of generate function
    public int currentRoomColumn;
    public int currentRoomRow;
    public string currentRoom;
    public int randomlyGeneratedDirection;
    public int lastDirection;
    public int maxRooms;
    public int safety = 0;
    public const string EMPTY_ROOM = "0000";

    enum RoomDirection
  {
    LEFT = 0,
    UP = 1,
    DOWN = 2,
    RIGHT = 3
  }


    public Board(){
        roomConnectionTypes.Add("0000", " ");
        roomConnectionTypes.Add("0001", "►");
        roomConnectionTypes.Add("0010", "▼");
        roomConnectionTypes.Add("0011", "╔");
        roomConnectionTypes.Add("0100", "▲");
        roomConnectionTypes.Add("0101", "╚");
        roomConnectionTypes.Add("0110", "║");
        roomConnectionTypes.Add("0111", "╠");
        roomConnectionTypes.Add("1000", "◄");
        roomConnectionTypes.Add("1001", "═");
        roomConnectionTypes.Add("1010", "╗");
        roomConnectionTypes.Add("1011", "╦");
        roomConnectionTypes.Add("1100", "╝");
        roomConnectionTypes.Add("1101", "╩");
        roomConnectionTypes.Add("1110", "╣");
        roomConnectionTypes.Add("1111", "╬");

        // lastDirection is initialized as -1 in order to 
        lastDirection = -1;
        maxRooms = 40;        
    }

    /// <summary>
    /// Fills map with "0000" value (empty rooms).
    /// </summary>
    public void fillBoardEmpty(){
        for(int row = 0; row < map.GetLength(1); row++){
            for(int column = 0; column < map.GetLength(0); column++){
                map[column, row] = EMPTY_ROOM;
            }
        }
    }

    public void placeFirstRoom(){
        // Generates a room with connections in all four direction in the middle of the map
        currentRoomColumn = map.GetLength(0) / 2;
        currentRoomRow = map.GetLength(1) / 2;
        currentRoom = "1111";

        placeRoom(currentRoom, currentRoomColumn, currentRoomRow);
        GD.Print("Room 1111 has spawn in Column (" + currentRoomColumn + ") and Row(" + currentRoomRow + ")");
    }

    /// <summary>
    /// Places a room on the map based on entered room binary and position
    /// </summary>
    /// <param name="room"></param>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public void placeRoom(string room, int column, int row){
        map[column, row] = room;
    }

    /// <summary>
    /// Fills entire board with completely randomized rooms.
    /// </summary>
    public void fillBoardRandom(){
        for(int row = 0; row < map.GetLength(1); row++){
            for(int column = 0; column < map.GetLength(0); column++){
                map[column, row] = getRandomNibble();
            }
        }
    }

    /// <summary>
    /// Returns map as a string to print out to console for visual representation.
    /// </summary>
    /// <returns></returns>
    public string getBoardString(){
        string tempString = "";
        for(int row = 0; row < map.GetLength(1); row++){
            for(int column = 0; column < map.GetLength(0); column++){
                tempString += roomConnectionTypes.GetValueOrDefault(map[column, row]);
            }
            tempString += "\n";
        }
        return tempString;
    }

    public override string ToString()
    {
        return getBoardString();
    }

    /// <summary>
    /// Returns a "1" or a "0" randomly.
    /// </summary>
    /// <returns></returns>
    public string getRandomBit(){
        int tempNum = random.Next(0, 2);
        return tempNum.ToString();
    }

    /// <summary>
    /// Returns a random binary string value for four digits values.
    /// </summary>
    /// <returns></returns>
    public string getRandomNibble(){
        return getRandomBit() + getRandomBit() + getRandomBit() + getRandomBit();
    }

    /// <summary>
    /// Creates a randomly generated map.
    /// </summary>
    public void addNextPartOfMap(){

            safety = 0;

            while(true){

                // Throws exception if looped too many times to prevent crashing
                safety++;
                if(safety > 50000){
                    throw new ArgumentException("Looped too many times");
                }

                // Selects a random direction to generate the next room in
                // If the room is at the edge of the map, it breaks out of loop to prevent another room from being generated
                randomlyGeneratedDirection = random.Next(0, 4);
                if(isAtEdgeOfMap(currentRoomColumn, currentRoomRow)){
                    break;
                }
                
                if(isGeneratedDirectionValid(randomlyGeneratedDirection, lastDirection, currentRoom, currentRoomColumn, currentRoomRow)){
                    break;
                }
            }

            GD.Print("Generating at directStart " + randomlyGeneratedDirection);

            int tempColumn = getIncrementColumnByBitPos(currentRoomColumn, randomlyGeneratedDirection);
            int tempRow = getIncrementRowByBitPos(currentRoomRow, randomlyGeneratedDirection);
            string tempRoom = "";
            
            // "Rerolls" up to ten times if next room is a dead end
            for(int i = 0; i < 10; i++){
                tempRoom = getGeneratedNextRoom(tempColumn, tempRow, randomlyGeneratedDirection);
                if(getAmountOfConnections(tempRoom) != 1){
                    break;
                }
            }

            // Should place a single possible room in a random direction
            GD.Print("Adding room: " + tempRoom + " to Column (" + tempColumn + ") and Row (" + tempRow + ")");
            placeRoom(tempRoom, tempColumn, tempRow);
            
            if(getAmountOfConnections(tempRoom) == 1){
                //break;
                GD.Print("STOP, ENDPOINT HAS BEEN REACHED!");
            }

            lastDirection = randomlyGeneratedDirection;
            currentRoom = tempRoom;
            currentRoomColumn = tempColumn;
            currentRoomRow = tempRow;
        
        
    }

    /// <summary>
    /// Returns true if all the following conditions are met:
    ///     - There is a connection in that direction from the current room selected
    ///     - The direction isn't going into the room previously generated
    ///     - The direction leads into an empty room or a room that connects
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="lastDirection"></param>
    /// <param name="room"></param>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool isGeneratedDirectionValid(int direction, int lastDirection, string room, int column, int row){
        return roomHasConnectionInDirection(room, direction) &&
                 lastDirection != getOppositeDirection(direction) &&
                 (String.Equals(map[getIncrementColumnByBitPos(column, direction),
                 getIncrementRowByBitPos(row, direction)], EMPTY_ROOM) || 
                 Char.Equals(map[getIncrementColumnByBitPos(column, direction),
                 getIncrementRowByBitPos(row, direction)][getOppositeDirection(direction)], '1'));
    }

    public string getGeneratedNextRoom(int column, int row, int direction){

        string tempRoom = "";
        
        GD.Print("I have gotten to this point");

        // Makes a dead end if at edge of map
            if(isAtEdgeOfMap(column, row)){
                for(int i = 0; i < 4; i++){
                    if(i == getOppositeDirection(direction)){
                        tempRoom += "1";
                    } else{
                        tempRoom += "0";
                    }
                
                }

                return tempRoom;
            }

            GD.Print("No way, me too!");


        string roomWithPossibleConnections = getRoomWithPossibleConnections(column, row);
        string roomWithMandatoryConnections = getRoomWithMandatoryConnections(column, row);
        

            for(int i = 0; i < 4; i++){
                if(i == getOppositeDirection(direction) || Char.Equals(roomWithMandatoryConnections[i], '1')){
                    tempRoom += "1";
                } else if(Char.Equals(roomWithPossibleConnections[i], '1')){
                    tempRoom += getRandomBit();
                    
                } else{
                    tempRoom += "0";
                }
                
            }

            return tempRoom;
    }

    /// <summary>
    /// Returns a bit position that is flipped. In terms of direction, it rotates it by 180 degrees.
    /// </summary>
    /// <param name="bitPos"></param>
    /// <returns></returns>
    public int getOppositeDirection(int bitPos){
        return Math.Abs(3 - bitPos);
    }

    /// <summary>
    /// Returns true if room from entered direction is valid to connect to. Also returns true if room is empty.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="bitPosDirection"></param>
    /// <returns></returns>
    public bool isConnectionValid(int column, int row, int bitPosDirection){
        int connectingRoomColumn = getIncrementColumnByBitPos(column, bitPosDirection);
        int connectingRoomRow = getIncrementRowByBitPos(row, bitPosDirection);

        /**
        GD.Print("Column (" + connectingRoomColumn + ")\nRow (" + connectingRoomRow + ")");
        GD.Print(map[connectingRoomColumn, connectingRoomRow]);
        **/
        //GD.Print(getOppositeDirection(bitPosDirection));
        return Char.Equals(map[connectingRoomColumn, connectingRoomRow][getOppositeDirection(bitPosDirection)], '1');
    }

    public bool isConnectionEmpty(int column, int row, int bitPosDirection){
        int connectingRoomColumn = getIncrementColumnByBitPos(column, bitPosDirection);
        int connectingRoomRow = getIncrementRowByBitPos(row, bitPosDirection);

        /**
        GD.Print("Column (" + connectingRoomColumn + ")\nRow (" + connectingRoomRow + ")");
        GD.Print(map[connectingRoomColumn, connectingRoomRow]);
        **/
        //GD.Print(getOppositeDirection(bitPosDirection));
        return Char.Equals(map[connectingRoomColumn, connectingRoomRow], EMPTY_ROOM);
    }

    /// <summary>
    /// Returns a binary string of a room that has the most possible valid connections at entered position.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public string getRoomWithPossibleConnections(int column, int row){
        string roomBinary = "";
        for(int i = 0; i < 4; i++){
            if(isConnectionEmpty(column, row, i)){
                roomBinary += "1";
            } else{
                roomBinary += "0";
            }
        }

        return roomBinary;
    
    }

    public string getRoomWithMandatoryConnections(int column, int row){
        string roomBinary = "";
        for(int i = 0; i < 4; i++){
            if(isConnectionValid(column, row, i)){
                roomBinary += "1";
            } else{
                roomBinary += "0";
            }
        }

        return roomBinary;
    
    }

    /// <summary>
    /// Returns incremented value of entered column moved in entered direction. 
    /// </summary>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public int getIncrementColumnByBitPos(int column, int direction){
        int tempColumn = column;
        if(direction == 3){
            tempColumn++;
        } else if(direction == 0){
            tempColumn--;
        }
        return tempColumn;
    }

    /// <summary>
    /// Returns incremented value of entered row moved in entered direction.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public int getIncrementRowByBitPos(int row, int direction){
        int tempRow = row;
        if(direction == 2){
            tempRow++;
        } else if(direction == 1){
            tempRow--;
        }
        return tempRow;
    }

    public int getAmountOfConnections(string room){
        int counter = 0;
        for(int i = 0; i < 4; i++){
            if(Char.Equals(room[i], '1')){
                counter++;
            }
        }
        return counter;
    }

    public bool isAtEdgeOfMap(int column, int row){
        if(column == 1 || column == map.GetLength(0) - 1 || row == 1 || row == map.GetLength(1) - 1){
            return true;
        }
        return false;
    }

    public bool roomHasConnectionInDirection(string room, int direction){
        return Char.Equals(room[direction], '1');
    }
}