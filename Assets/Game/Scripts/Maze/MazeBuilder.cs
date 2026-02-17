using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    private Maze maze;
    private Cell[,] cellArray;

    [SerializeField] int numRows = 5;
    [SerializeField] int numColumns = 5;
    [SerializeField] float wallLength = 10f;
    [SerializeField] float wallDepth = 1f;
    [SerializeField] GameObject wallPrefab;

    private void Start()
    {
        maze = new Maze(numRows, numColumns);
        cellArray = maze.cellArray;

        PrintMaze();
        Debug.Log("Instantiating maze walls");

        /*// Instantiating top wall of maze
        for (int i = 0; i < numRows; i++)
        {
            Instantiate(wallPrefab,
                        new Vector3((1 * (wallLength + 1)) + wallDepth,
                                    0,
                                    (i * (wallLength + 1)) + (wallLength / 2)),
                        Quaternion.identity);
        }

        // Instantiating right wall of maze
        for (int i = 0; i < numColumns; i++)
        {
            Instantiate(wallPrefab,
                        new Vector3((i * (wallLength + 1)) - wallDepth,
                                    0,
                                    (1 * (wallLength + 1)) - (wallLength / 2)),
                        Quaternion.identity * Quaternion.AngleAxis(90, Vector3.up));
        }*/

        // Instantiating full maze
        for (int x = 0; x < numRows; x++)
        {
            for (int z = 0; z < numColumns; z++)
            {
                Cell cell = cellArray[x, z];
                
                if (cell.bottomWall == true)
                {
                    Instantiate(wallPrefab,
                        new Vector3((x * (wallLength + 1)) + wallDepth,
                                    0,
                                    (z * (wallLength + 1)) + (wallLength / 2)),
                        Quaternion.identity);
                }

                if (cell.leftWall == true)
                {
                    Instantiate(wallPrefab,
                        new Vector3((x * (wallLength + 1)) - (wallLength / 2),
                                    0,
                                    (z * (wallLength + 1)) - wallDepth),
                        Quaternion.identity * Quaternion.AngleAxis(90, Vector3.up));
                }
            }
        }
    }





    public void PrintMaze()
    {
        string maze_str = "";
        // Printing top of maze
        for (int i = 0; i < numColumns; i++)
        {
            maze_str += " _";
        }
        maze_str += "\n";

        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numColumns; c++)
            {
                Cell cell = cellArray[r, c];
                if (cell.leftWall == true && cell.bottomWall == true)
                    maze_str += "|_";
                else if (cell.leftWall == false && cell.bottomWall == true)
                    maze_str += " _";
                else if (cell.leftWall == true && cell.bottomWall == false)
                    maze_str += "| ";
                else
                    maze_str += "  ";
            }
            maze_str += "|\n";
        }
        maze_str += "\n\n";

        Debug.Log(maze_str);
    }
}
