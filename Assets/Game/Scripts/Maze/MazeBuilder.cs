using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    [System.Serializable]
    public struct Dimension
    {
        public int width;
        public int height;

        public Dimension(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    private Maze maze;
    private Cell[,] cellArray;

    [Header("Maze Settings")]
    [SerializeField] int numRows = 5;
    [SerializeField] int numColumns = 5;
    [Header("Rooms")]
    [SerializeField] List<Dimension> roomDimensions;
    [Header("Dimensions")]
    [SerializeField] float wallLength = 9f;
    [SerializeField] float wallHeight = 5f;
    [SerializeField] float pillarDiameter = 1f;
    [SerializeField] float pillarHeight = 5f;
    [Header("Prefabs")]
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject pillarPrefab;

    private void Awake()
    {
        roomDimensions = new List<Dimension>();
        maze = new Maze(numRows, numColumns);
        cellArray = maze.cellArray;
    }

    private void Start()
    {
        //PrintMaze();

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
                
                if (cell.bottom.exists == true)
                {
                    obj = Instantiate(wallPrefab,
                        new Vector3((x * (wallLength + pillarDiameter) + (pillarDiameter / 2) + wallLength),
                                    (wallHeight / 2),
                                    (z * (wallLength + pillarDiameter) + (wallLength / 2) + pillarDiameter)),
                        Quaternion.identity);
                    obj.transform.parent = this.transform;
                }

                if (cell.left.exists == true)
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
                        leftCell = cellArray[x, z - 1];
                    }
                    Cell bottomCell = null;
                    if (x + 1 <= numRows - 1)
                    {
                        bottomCell = cellArray[x + 1, z];
                    }

                    if (cell.bottom.exists && cell.left.exists)
                        SpawnPillar(x, z);
                    else if (bottomCell != null && (cell.left.exists && bottomCell.left.exists))
                        SpawnPillar(x, z);
                    else if (bottomCell != null && (cell.bottom.exists && bottomCell.left.exists))
                        SpawnPillar(x, z);
                    else if (leftCell != null && (cell.bottom.exists && leftCell.bottom.exists))
                        SpawnPillar(x, z);
                    else if (leftCell != null && (cell.left.exists && leftCell.bottom.exists))
                        SpawnPillar(x, z);
                    else if (leftCell != null && bottomCell != null)
                        if (leftCell.bottom.exists && bottomCell.left.exists)
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
                if (cell.left.exists == true && cell.bottom.exists == true)
                    maze_str += "|_";
                else if (cell.left.exists == false && cell.bottom.exists == true)
                    maze_str += " _";
                else if (cell.left.exists == true && cell.bottom.exists == false)
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
