using UnityEngine;

public class Maze
{
    public Cell[,] cellArray;
    DisjointSet dsMaze;

    public int numRows;
    public int numCols;
    public int numCells;

    public Maze(int numRows, int numCols)
    {
        cellArray = new Cell[numRows, numCols];
        this.numRows = numRows;
        this.numCols = numCols;
        numCells = numRows * numCols;

        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                cellArray[r, c] = new Cell();
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
            // grab random cell, num between 0 and n-1
            int cellNum = Random.Range(0, numCells);
            int row = cellNum / numRows;
            int col = cellNum % numCols;
            int adjRow = -1;
            int adjCol = -1;

            /** Randomly pick a wall in the maze and remove it, then union the sets of the two cells */

            // cell is a border cell
            if (row == 0 || col == 0 || row == numRows - 1 || col == numCols - 1)
            {
                /**  CORNER CELL CASES  **/
                if (row == 0 && col == 0)  // top left corner
                {
                    // 50% chance (1 or 2) choice between right and bottom cell
                    int rand = Random.Range(1, 3);
                    if (rand == 1) // right cell
                    {
                        adjRow = row;
                        adjCol = col + 1;
                    }
                    else // bottom cell
                    {
                        adjRow = row + 1;
                        adjCol = col;
                    }
                }
                else if (row == 0 && col == numCols - 1)  // top right corner
                {
                    int rand = Random.Range(1, 3);
                    if (rand == 1) // bottom cell
                    {
                        adjRow = row + 1;
                        adjCol = col;
                    }
                    else // left cell
                    {
                        adjRow = row;
                        adjCol = col - 1;
                    }
                }
                else if (row == numRows - 1 && col == 0)  // bottom left corner
                {
                    int rand = Random.Range(1, 3);
                    if (rand == 1) // top cell
                    {
                        adjRow = row - 1;
                        adjCol = col;
                    }
                    else // right cell
                    {
                        adjRow = row;
                        adjCol = col + 1;
                    }
                }
                else if (row == numRows - 1 && col == numCols - 1)  // bottom right corner
                {
                    int rand = Random.Range(1, 3);
                    if (rand == 1) // left cell
                    {
                        adjRow = row;
                        adjCol = col - 1;
                    }
                    else // top cell
                    {
                        adjRow = row - 1;
                        adjCol = col;
                    }
                }

                /**  BORDER CELL CASES  **/
                else if (col == 0)  // left side
                {
                    // 33% chance (1 or 2 or 3) choice between top, right, and bottom cell
                    int rand = Random.Range(1, 4);
                    if (rand == 1) // top cell
                    {
                        adjRow = row - 1;
                        adjCol = col;
                    }
                    else if (rand == 2) // right cell
                    {
                        adjRow = row;
                        adjCol = col + 1;
                    }
                    else // bottom cell
                    {
                        adjRow = row + 1;
                        adjCol = col;
                    }
                }
                else if (col == numCols - 1)  // right side
                {
                    int rand = Random.Range(1, 4);
                    if (rand == 1) // bottom cell
                    {
                        adjRow = row + 1;
                        adjCol = col;
                    }
                    else if (rand == 2) // left cell
                    {
                        adjRow = row;
                        adjCol = col - 1;
                    }
                    else // top cell
                    {
                        adjRow = row - 1;
                        adjCol = col;
                    }
                }
                else if (row == 0)  // top
                {
                    int rand = Random.Range(1, 4);
                    if (rand == 1) // right cell
                    {
                        adjRow = row;
                        adjCol = col + 1;
                    }
                    else if (rand == 2) // bottom cell
                    {
                        adjRow = row + 1;
                        adjCol = col;
                    }
                    else // left cell
                    {
                        adjRow = row;
                        adjCol = col - 1;
                    }
                }
                else if (row == numRows - 1)  // bottom
                {
                    int rand = Random.Range(1, 4);
                    if (rand == 1) // left cell
                    {
                        adjRow = row;
                        adjCol = col - 1;
                    }
                    else if (rand == 2) // top cell
                    {
                        adjRow = row - 1;
                        adjCol = col;
                    }
                    else // right cell
                    {
                        adjRow = row;
                        adjCol = col + 1;
                    }
                }
            }
            // Cell is an inner cell
            else
            {
                // 25% chance (1, 2, 3, or 4) choice between top, bottom, left, and right cells
                int rand = Random.Range(1, 5);
                if (rand == 1) // top cell
                {
                    adjRow = row - 1;
                    adjCol = col;
                }
                else if (rand == 2) // right cell
                {
                    adjRow = row;
                    adjCol = col + 1;
                }
                else if (rand == 3) // bottom cell
                {
                    adjRow = row + 1;
                    adjCol = col;
                }
                else // left cell
                {
                    adjRow = row;
                    adjCol = col - 1;
                }
            }

            if (adjRow == -1 || adjCol == -1)
            {
                Debug.LogError("row or col of adj cell should not be -1");
            }

            int adjNum = (numCols) * (adjRow) + (adjCol);
            int cellSet = dsMaze.find(cellNum);
            int adjSet = dsMaze.find(adjNum);

            // if cells are not in same set, union and remove wall
            if (cellSet != adjSet)
            {
                dsMaze.union(cellSet, adjSet);
                if (row + 1 == adjRow) // adj cell is cell below
                    cellArray[row, col].bottomWall = false;
                else if (row - 1 == adjRow) // adj cell is cell above
                    cellArray[adjRow, adjCol].bottomWall = false;
                else if (col + 1 == adjCol) // adj cell is cell to right
                    cellArray[adjRow, adjCol].leftWall = false;
                else if (col - 1 == adjCol) // adj cell is cell to left
                    cellArray[row, col].leftWall = false;

                numUnions++;
                if (numUnions == numCells - 1)
                    mazeBuilt = true;
            }
        }
    }
}
