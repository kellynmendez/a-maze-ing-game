using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maze
{
    public Cell[,] cellArray;
    public int numRows;
    public int numCols;
    public int numCells;

    private DisjointSet dsMaze;
    private List<Wall> walls;

    public Maze(int numRows, int numCols)
    {
        cellArray = new Cell[numRows, numCols];
        walls = new List<Wall>();
        this.numRows = numRows;
        this.numCols = numCols;
        numCells = numRows * numCols;

        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                Cell currCell = new Cell(r, c);
                cellArray[r, c] = currCell;
                if (!(c == 0 && r == numRows - 1))
                {
                    if (c == 0)
                    {
                        walls.Add(currCell.bottom);
                    }
                    else if (r == numRows - 1)
                    {
                        walls.Add(currCell.left);
                    }
                    else
                    {
                        walls.Add(currCell.left);
                        walls.Add(currCell.bottom);
                    }
                }
            }
        }

        dsMaze = new DisjointSet(numCells);

        // Build out walls of the maze
        BuildMaze();
    }

    private void BuildMaze()
    {
        bool mazeBuilt = false;
        int numUnions = 0;

        while(!mazeBuilt)
        {
            // grab random wall
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
            int cellSet = dsMaze.find(cellNum);
            int adjNum = numCols * adjCell.row + adjCell.col;
            int adjSet = dsMaze.find(adjNum);

            // if cells are not in same set, union and remove wall
            if (cellSet != adjSet)
            {
                dsMaze.union(cellSet, adjSet);
                if (thisCell.row + 1 == adjCell.row) // adj cell is cell below
                    cellArray[thisCell.row, thisCell.col].bottom.exists = false;
                else if (thisCell.row - 1 == adjCell.row) // adj cell is cell above
                    cellArray[adjCell.row, adjCell.col].bottom.exists = false;
                else if (thisCell.col + 1 == adjCell.col) // adj cell is cell to right
                    cellArray[adjCell.row, adjCell.col].left.exists = false;
                else if (thisCell.col - 1 == adjCell.col) // adj cell is cell to left
                    cellArray[thisCell.row, thisCell.col].left.exists = false;

                numUnions++;
                if (numUnions == numCells - 1)
                    mazeBuilt = true;
            }
        }
    }
}
