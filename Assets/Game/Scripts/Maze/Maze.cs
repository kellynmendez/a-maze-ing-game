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
    private List<Room> rooms;

    public Maze(int numRows, int numCols, List<Room> rooms)
    {
        this.numRows = numRows;
        this.numCols = numCols;
        this.rooms = rooms;
        cellArray = new Cell[numRows, numCols];
        walls = new List<Wall>();
        numCells = numRows * numCols;

        InitializeCellsAndWalls();
        dsMaze = new DisjointSet(numCells);

        // Build the maze including rooms
        BuildMaze();
    }

    /**
     * Initialize cell array with all cells, then add all walls that are not
     * the full maze's border walls to the walls list
     */
    private void InitializeCellsAndWalls()
    {
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
            }
        }
        return true;
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

        // Randomly pick one wall as a door
        Wall doorWall = perimeterWalls[Random.Range(0, perimeterWalls.Count)];
        doorWall.exists = false;

        // Union the room to the maze through the door
        ConnectDoorToMaze(doorWall, ref numUnions);

        // Remove all other perimeter walls from walls list (so they cannot be
        //  knocked down when building out the rest of the maze)
        foreach (Wall w in perimeterWalls)
        {
            if (w != doorWall)
                walls.Remove(w);
        }

        Debug.Log($"Building room {room.room} at r={startRow}, c={startCol}");
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
     * Randomly finds a valid top left cell for a room
     */
    private Cell FindRoomPlacement(Room room)
    {
        List<Cell> availableCells = new List<Cell>();
        for (int r = 0; r < numRows; r++)
            for (int c = 0; c < numCols; c++)
                availableCells.Add(cellArray[r, c]);

        Cell randCell = null;
        while (randCell == null)
        {
            int randIndex = Random.Range(0, availableCells.Count);
            Cell checkCell = availableCells[randIndex];

            if (CheckCellAvailability(room.z, room.x, checkCell.row, checkCell.col))
            {
                randCell = checkCell;
                break;
            }
            else
            {
                availableCells.RemoveAt(randIndex);
            }
        }

        return randCell;
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

            if (cellSet != adjSet)
            {
                dsMaze.union(cellSet, adjSet);
                walls.Remove(randWall);

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

    private void BuildMaze()
    {
        int numUnions = 0;

        // Build rooms first
        foreach (Room room in rooms)
        {
            Cell topLeft = FindRoomPlacement(room);
            CarveRoom(room, topLeft, ref numUnions);
        }

        // Then build out the rest of the maze
        BuildRemainingMaze(ref numUnions);
    }

}