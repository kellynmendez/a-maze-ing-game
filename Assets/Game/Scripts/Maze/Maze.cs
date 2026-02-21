using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    public Cell[,] cellArray;
    public int numRows;
    public int numCols;
    public int numCells;

    private DisjointSet dsMaze;
    private List<Wall> walls;
    private List<Wall> doors;
    private List<Cell> availableCells;
    private List<Room> rooms;

    public Maze(int numRows, int numCols, List<Room> rooms)
    {
        this.numRows = numRows;
        this.numCols = numCols;
        this.rooms = rooms;

        bool success = false;
        int attempts = 0;
        int maxAttempts = 30;
        while (!success && attempts < maxAttempts)
        {
            attempts++;

            InitializeGridAndVars();
            success = BuildMaze();
        }

        if (!success)
        {
            Debug.LogError("Maze generation failed after max attempts!");
        }
    }

    private void InitializeGridAndVars()
    {
        cellArray = new Cell[numRows, numCols];
        walls = new List<Wall>();
        doors = new List<Wall>();
        numCells = numRows * numCols;

        // Initialize cell array w base cells
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                Cell currCell = new Cell(r, c);
                cellArray[r, c] = currCell;

                if (!(c == 0 && r == numRows - 1))
                {
                    if (c == 0)
                        walls.Add(currCell.bottom);
                    else if (r == numRows - 1)
                        walls.Add(currCell.left);
                    else
                    {
                        walls.Add(currCell.left);
                        walls.Add(currCell.bottom);
                    }
                }
            }
        }

        // List of cells that are available for carving out rooms
        availableCells = new List<Cell>();
        for (int r = 0; r < numRows; r++)
            for (int c = 0; c < numCols; c++)
                availableCells.Add(cellArray[r, c]);

        dsMaze = new DisjointSet(numCells);
    }

    /**
     * Carve the room out the maze!
     */
    private void CarveRoom(Room room, Cell topLeft, ref int numUnions)
    {
        int startRow = topLeft.row;
        int startCol = topLeft.col;
        int endRow = startRow + room.x - 1;
        int endCol = startCol + room.z - 1;

        for (int r = startRow; r <= endRow; r++)
        {
            for (int c = startCol; c <= endCol; c++)
            {
                // Get and add the next cell in the room to the room
                Cell temp = cellArray[r, c];
                temp.available = false;

                int roomIndex = numCols * startRow + startCol;
                int cellIndex = numCols * r + c;
                int roomSet = dsMaze.find(roomIndex);
                int cellSet = dsMaze.find(cellIndex);

                if (roomSet != cellSet)
                {
                    dsMaze.union(roomSet, cellSet);
                    numUnions++;
                }

                // Remove internal walls
                if (r > startRow)
                    cellArray[r - 1, c].bottom.exists = false;
                if (c > startCol)
                    cellArray[r, c].left.exists = false;
            }
        }

        // Get perimeter walls so a door can be made into the room
        List<Wall> perimeterWalls = GetPerimeterWalls(startRow, startCol, endRow, endCol);

        // Do not want the door to be a wall that two rooms share
        List<Wall> validDoors = new List<Wall>();
        foreach (Wall w in perimeterWalls)
        {
            Cell adjCell;
            if (w.side == "L")
                adjCell = cellArray[w.parent.row, w.parent.col - 1];
            else
                adjCell = cellArray[w.parent.row + 1, w.parent.col];

            if (adjCell.available)
                validDoors.Add(w);
        }

        if (validDoors.Count > 0)
        {
            Wall doorWall = validDoors[Random.Range(0, validDoors.Count)];
            doorWall.exists = false;

            // Union the room to the rest of the maze
            ConnectDoorToMaze(doorWall, ref numUnions);
            doors.Add(doorWall);

            // Do not want the perimeter walls to be options to knock 
            //  down when building out the rest of the maze
            foreach (Wall w in perimeterWalls)
            {
                if (w != doorWall)
                    walls.Remove(w);
            }
        }

        //Debug.Log($"Building room {room.room} at r={startRow}, c={startCol}");
    }

    private List<Wall> GetPerimeterWalls(int startRow, int startCol, int endRow, int endCol)
    {
        List<Wall> perimeterWalls = new List<Wall>();

        // top
        if (startRow > 0)
            for (int c = startCol; c <= endCol; c++)
                perimeterWalls.Add(cellArray[startRow - 1, c].bottom);

        // bottom
        if (endRow < numRows - 1)
            for (int c = startCol; c <= endCol; c++)
                perimeterWalls.Add(cellArray[endRow, c].bottom);

        // left
        if (startCol > 0)
            for (int r = startRow; r <= endRow; r++)
                perimeterWalls.Add(cellArray[r, startCol].left);

        // right
        if (endCol < numCols - 1)
            for (int r = startRow; r <= endRow; r++)
                perimeterWalls.Add(cellArray[r, endCol + 1].left);

        return perimeterWalls;
    }

    private void ConnectDoorToMaze(Wall doorWall, ref int numUnions)
    {
        // Get adjacent cell
        Cell roomCell = doorWall.parent;
        Cell adjCell;
        if (doorWall.side == "L")
        {
            adjCell = cellArray[roomCell.row, roomCell.col - 1];
        }
        else
        {
            adjCell = cellArray[roomCell.row + 1, roomCell.col];
        }

        // Union the room to rest of maze
        int roomIndex = numCols * roomCell.row + roomCell.col;
        int adjIndex = numCols * adjCell.row + adjCell.col;
        int roomSet = dsMaze.find(roomIndex);
        int adjSet = dsMaze.find(adjIndex);
        if (roomSet != adjSet)
        {
            dsMaze.union(roomSet, adjSet);
            numUnions++;
        }
    }

    /**
     * Checks if, given a room's dimensions and a potential cell that would be the
     * top left cell of the room, whether or not that cell is a valid place to 
     * make the room.
     */
    private bool CheckCellAvailability(int roomZ, int roomX, int cellR, int cellC)
    {
        // if the cell's row or columns plus size of room exceeds the maze bounds, it is not valid
        if (cellR + roomX > numRows || cellC + roomZ > numCols)
            return false;

        // checking that all cells in the potential room space are available (not taken up by other rooms)
        for (int r = cellR; r < cellR + roomX; r++)
        {
            for (int c = cellC; c < cellC + roomZ; c++)
            {
                if (!(cellArray[r, c].available))
                    return false;

                // the new room cannot touch an existing door
                foreach (Wall door in doors)
                {
                    Cell doorCell = door.parent;
                    Cell outsideCell;

                    if (door.side == "L")
                        outsideCell = cellArray[doorCell.row, doorCell.col - 1];
                    else
                        outsideCell = cellArray[doorCell.row + 1, doorCell.col];

                    if (cellArray[r, c] == outsideCell)
                        return false;
                }
            }
        }

        return true;
    }

    /**
     * Randomly finds a valid top left cell for a room
     */
    private Cell FindRoomPlacement(Room room)
    {
        while (availableCells.Count > 0)
        {
            int randIndex = Random.Range(0, availableCells.Count);
            Cell checkCell = availableCells[randIndex];

            if (CheckCellAvailability(room.z, room.x, checkCell.row, checkCell.col))
            {
                return checkCell;
            }
            else
            {
                availableCells.RemoveAt(randIndex);
            }
        }

        Debug.LogWarning("No valid placement found for room.");
        return null;
    }

    /**
     * Build the rest of the maze using the disjoint set maze gen algorithm
     */
    private void BuildRemainingMaze(ref int numUnions)
    {
        bool mazeBuilt = false;

        while (!mazeBuilt && walls.Count > 0)
        {
            int rand = Random.Range(0, walls.Count);
            Wall randWall = walls[rand];

            Cell thisCell = randWall.parent;
            Cell adjCell;
            if (randWall.side == "L")
            {
                adjCell = cellArray[thisCell.row, thisCell.col - 1];
            }
            else
            {
                adjCell = cellArray[thisCell.row + 1, thisCell.col];
            }

            int cellNum = numCols * thisCell.row + thisCell.col;
            int adjNum = numCols * adjCell.row + adjCell.col;
            int cellSet = dsMaze.find(cellNum);
            int adjSet = dsMaze.find(adjNum);

            walls.Remove(randWall);

            if (cellSet != adjSet)
            {
                dsMaze.union(cellSet, adjSet);

                if (thisCell.row + 1 == adjCell.row)
                    cellArray[thisCell.row, thisCell.col].bottom.exists = false;
                else if (thisCell.row - 1 == adjCell.row)
                    cellArray[adjCell.row, adjCell.col].bottom.exists = false;
                else if (thisCell.col + 1 == adjCell.col)
                    cellArray[adjCell.row, adjCell.col].left.exists = false;
                else if (thisCell.col - 1 == adjCell.col)
                    cellArray[thisCell.row, thisCell.col].left.exists = false;

                numUnions++;
                if (numUnions == numCells - 1)
                    mazeBuilt = true;
            }
        }
    }

    private bool BuildMaze()
    {
        int numUnions = 0;

        // The first room needs to be at the center of the maze (player start location)
        if (rooms.Count > 0)
        {
            Room firstRoom = rooms[0];
            int centerRow = numRows / 2 - firstRoom.x / 2;
            int centerCol = numCols / 2 - firstRoom.z / 2;

            Cell topLeft = cellArray[centerRow, centerCol];
            topLeft.room = firstRoom;
            CarveRoom(firstRoom, topLeft, ref numUnions);
        }

        // Build rest of rooms
        for (int i = 1; i < rooms.Count; i++)
        {
            Room room = rooms[i];
            Cell topLeft = FindRoomPlacement(room);

            if (topLeft == null)
            {
                return false;
            }

            topLeft.room = room;
            CarveRoom(room, topLeft, ref numUnions);
        }

        // Then build out the rest of the maze
        BuildRemainingMaze(ref numUnions);

        return true;
    }

}