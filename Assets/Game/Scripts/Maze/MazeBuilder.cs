using System.Runtime.CompilerServices;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    private Maze maze;
    private Cell[,] cellArray;

    [SerializeField] int numRows = 5;
    [SerializeField] int numColumns = 5;
    [SerializeField] float wallLength = 9f;
    [SerializeField] float wallDepth = 1f;
    [SerializeField] float wallHeight = 5f;
    [SerializeField] float pillarDiameter = 1f;
    [SerializeField] float pillarHeight = 5f;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject pillarPrefab;

    private void Start()
    {
        maze = new Maze(numRows, numColumns);
        cellArray = maze.cellArray;

        PrintMaze();

        // Instantiating top left pillar
        GameObject obj = Instantiate(pillarPrefab,
                            new Vector3(-(pillarDiameter / 2), (pillarHeight / 2), (pillarDiameter / 2)),
                            Quaternion.identity);
        obj.transform.parent = this.transform;

        // Instantiating top wall of maze
        for (int i = 0; i < numRows; i++)
        {
            obj = Instantiate(pillarPrefab,
                        new Vector3(-(pillarDiameter / 2),
                                    (pillarHeight / 2),
                                    (((i + 1) * wallLength) + ((i + 1) * pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.identity);
            obj.transform.parent = this.transform;
            obj = Instantiate(wallPrefab,
                        new Vector3(-(pillarDiameter / 2),
                                    (wallHeight / 2),
                                    (i * (wallLength + pillarDiameter) + (wallLength / 2) + pillarDiameter)),
                        Quaternion.identity);
            obj.transform.parent = this.transform;
        }

        // Instantiating right wall of maze
        for (int i = 0; i < numColumns; i++)
        {
            obj = Instantiate(wallPrefab,
                        new Vector3((i * (wallLength + pillarDiameter) + (wallLength / 2)),
                                    (wallHeight / 2),
                                    (numColumns * (wallLength + pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.identity * Quaternion.AngleAxis(90, Vector3.up));
            obj.transform.parent = this.transform;
            obj = Instantiate(pillarPrefab,
                        new Vector3((((i + 1) * wallLength) + (i * pillarDiameter) + (pillarDiameter / 2)),
                                    (pillarHeight / 2),
                                    (numColumns * (wallLength + pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.identity * Quaternion.AngleAxis(90, Vector3.up));
            obj.transform.parent = this.transform;
        }

        // Instantiating maze walls
        for (int x = 0; x < numRows; x++)
        {
            for (int z = 0; z < numColumns; z++)
            {
                Cell cell = cellArray[x, z];
                
                if (cell.bottomWall == true)
                {
                    obj = Instantiate(wallPrefab,
                        new Vector3((x * (wallLength + pillarDiameter) + (pillarDiameter / 2) + wallLength),
                                    (wallHeight / 2),
                                    (z * (wallLength + pillarDiameter) + (wallLength / 2) + pillarDiameter)),
                        Quaternion.identity);
                    obj.transform.parent = this.transform;
                }

                if (cell.leftWall == true)
                {
                    obj = Instantiate(wallPrefab,
                        new Vector3((x * (wallLength + pillarDiameter) + (wallLength / 2)),
                                    (wallHeight / 2),
                                    (z * (wallLength + pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.identity * Quaternion.AngleAxis(90, Vector3.up));
                    obj.transform.parent = this.transform;
                }
            }
        }

        // Instantiating pillars

        void SpawnPillar(int x, int z)
        {
            obj = Instantiate(pillarPrefab,
                        new Vector3(((x * wallLength) + (x * pillarDiameter) + wallLength + (pillarDiameter / 2)),
                                    (pillarHeight / 2),
                                    ((z * wallLength) + (z * pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.identity);
            obj.transform.parent = this.transform;
        }

        for (int x = 0; x < numRows; x++)
        {
            for (int z = 0; z < numColumns; z++)
            {
                Cell cell = cellArray[x, z];

                if (z == 0 || x == numRows - 1)
                    SpawnPillar(x, z);
                else
                {
                    // Grab adjacent cells
                    Cell leftCell = null;
                    if (z - 1 >= 0)
                    {
                        Debug.Log("adding left cell");
                        leftCell = cellArray[x, z - 1];
                    }
                    Cell bottomCell = null;
                    if (x + 1 <= numRows - 1)
                    {
                        Debug.Log("adding bottom cell");
                        bottomCell = cellArray[x + 1, z];
                    }

                    if (cell.bottomWall && cell.leftWall)
                        SpawnPillar(x, z);
                    else if (bottomCell != null && (cell.leftWall && bottomCell.leftWall))
                        SpawnPillar(x, z);
                    else if (bottomCell != null && (cell.bottomWall && bottomCell.leftWall))
                        SpawnPillar(x, z);
                    else if (leftCell != null && (cell.bottomWall && leftCell.bottomWall))
                        SpawnPillar(x, z);
                    else if (leftCell != null && (cell.leftWall && leftCell.bottomWall))
                        SpawnPillar(x, z);
                    else if (leftCell != null && bottomCell != null)
                        if (leftCell.bottomWall && bottomCell.leftWall)
                            SpawnPillar(x, z);
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
