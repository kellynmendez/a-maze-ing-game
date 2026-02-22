using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public GameObject room;
    public int x;
    public int z;

    public Room(GameObject room, int x, int z)
    {
        this.room = room;
        this.x = x;
        this.z = z;
    }
}

public class MazeBuilder : MonoBehaviour
{

    private Maze maze;
    private Cell[,] cellArray;
    private GameObject door;

    [Header("Maze Settings")]
    [SerializeField] int numRows = 5;
    [SerializeField] int numColumns = 5;
    [Header("Rooms")]
    [SerializeField] List<Room> rooms;
    [Header("Dimensions")]
    [SerializeField] float wallLength = 9f;
    [SerializeField] float pillarDiameter = 1f;
    [Header("Prefabs")]
    [SerializeField] List<GameObject> wallPrefabs;
    [SerializeField] List<GameObject> pillarPrefabs;
    [SerializeField] GameObject carrotPrefab;

    private void Awake()
    {
        door = FindObjectsByType<Door>(FindObjectsSortMode.None)[0].gameObject;
        maze = new Maze(numRows, numColumns, rooms);
        cellArray = maze.cellArray;
    }

    private void Start()
    {
        //PrintMaze();

        InstantiateWallsAndPillars();
        BuildRooms();
        SpawnCarrots();
    }

    private void InstantiateWallsAndPillars()
    {
        GameObject obj;
        // Instantiating top wall of maze
        for (int i = 0; i < numRows; i++)
        {
            int rand = Random.Range(0, pillarPrefabs.Count);
            GameObject pillarPrefab = pillarPrefabs[rand];
            obj = Instantiate(pillarPrefab,
                        new Vector3(-(pillarDiameter / 2),
                                    0,
                                    (((i + 1) * wallLength) + ((i + 1) * pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.identity);
            obj.transform.parent = this.transform;

            rand = Random.Range(0, wallPrefabs.Count);
            GameObject wallPrefab = wallPrefabs[rand];
            obj = Instantiate(wallPrefab,
                        new Vector3(-(pillarDiameter / 2),
                                    0,
                                    (i * (wallLength + pillarDiameter) + (wallLength / 2) + pillarDiameter)),
                        Quaternion.Euler(0, Random.Range(0, 2) * 180f, 0));
            obj.transform.parent = this.transform;
        }

        // Instantiating right wall of maze
        for (int i = 0; i < numColumns; i++)
        {
            int rand = Random.Range(0, wallPrefabs.Count);
            GameObject wallPrefab = wallPrefabs[rand];
            obj = Instantiate(wallPrefab,
                        new Vector3((i * (wallLength + pillarDiameter) + (wallLength / 2)),
                                    0,
                                    (numColumns * (wallLength + pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.Euler(0, Random.Range(0, 2) * 180f + 90f, 0));
            obj.transform.parent = this.transform;

            rand = Random.Range(0, pillarPrefabs.Count);
            GameObject pillarPrefab = pillarPrefabs[rand];
            obj = Instantiate(pillarPrefab,
                        new Vector3((((i + 1) * wallLength) + (i * pillarDiameter) + (pillarDiameter / 2)),
                                    0,
                                    (numColumns * (wallLength + pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.identity);
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
                    if (cell.bottom.isDoor == true)
                    {
                        door.transform.position = new Vector3((x * (wallLength + pillarDiameter) + (pillarDiameter / 2) + wallLength),
                                    0,
                                    (z * (wallLength + pillarDiameter) + (wallLength / 2) + pillarDiameter));

                        door.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else
                    {
                        int rand = Random.Range(0, wallPrefabs.Count);
                        GameObject wallPrefab = wallPrefabs[rand];
                        obj = Instantiate(wallPrefab,
                        new Vector3((x * (wallLength + pillarDiameter) + (pillarDiameter / 2) + wallLength),
                                    0,
                                    (z * (wallLength + pillarDiameter) + (wallLength / 2) + pillarDiameter)),
                        Quaternion.Euler(0, Random.Range(0, 2) * 180f, 0));
                        obj.transform.parent = this.transform;
                    }
                }

                if (cell.left.exists == true)
                {
                    if (cell.left.isDoor == true)
                    {
                        door.transform.position = new Vector3((x * (wallLength + pillarDiameter) + (wallLength / 2)),
                                    0,
                                    (z * (wallLength + pillarDiameter) + (pillarDiameter / 2)));
                        door.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        int rand = Random.Range(0, wallPrefabs.Count);
                        GameObject wallPrefab = wallPrefabs[rand];
                        obj = Instantiate(wallPrefab,
                        new Vector3((x * (wallLength + pillarDiameter) + (wallLength / 2)),
                                    0,
                                    (z * (wallLength + pillarDiameter) + (pillarDiameter / 2))),
                        Quaternion.Euler(0, Random.Range(0, 2) * 180f + 90f, 0));
                        obj.transform.parent = this.transform;
                    }
                }
            }
        }

        // Instantiating top left pillar
        int randPillar = Random.Range(0, pillarPrefabs.Count);
        GameObject cornerPillar = pillarPrefabs[randPillar];
        obj = Instantiate(cornerPillar,
                            new Vector3(-(pillarDiameter / 2), 0, (pillarDiameter / 2)),
                            Quaternion.identity);
        obj.transform.parent = this.transform;

        // Instantiating pillars
        void SpawnPillar(int x, int z)
        {
            int rand = Random.Range(0, pillarPrefabs.Count);
            GameObject pillarPrefab = pillarPrefabs[rand];
            obj = Instantiate(pillarPrefab,
                        new Vector3(((x * wallLength) + (x * pillarDiameter) + wallLength + (pillarDiameter / 2)),
                                    0,
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

    private void BuildRooms()
    {
        foreach (Cell cell in cellArray)
        {
            if (cell.room != null)
            {
                GameObject room = Instantiate(cell.room.room,
                        new Vector3(
                            ((cell.row * wallLength) + (cell.row * pillarDiameter) - (pillarDiameter / 2) + ((wallLength * cell.room.x) / 2) + ((pillarDiameter * cell.room.x) / 2)),
                            0,
                            ((cell.col * wallLength) + (cell.col * pillarDiameter) + (pillarDiameter / 2) + ((wallLength * cell.room.z) / 2) + ((pillarDiameter * cell.room.z) / 2))),
                        Quaternion.identity);
                room.transform.parent = this.transform;
            }
        }
    }

    private void SpawnCarrots()
    {
        foreach (Cell cell in cellArray)
        {
            if (cell.available)
            {
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    GameObject carrot = Instantiate(carrotPrefab,
                        new Vector3(((cell.row * wallLength) + (cell.row * pillarDiameter) - (pillarDiameter / 2) + (wallLength / 2)),
                                0,
                                ((cell.col * wallLength) + (cell.col * pillarDiameter) + (pillarDiameter / 2) + (wallLength / 2))),
                            Quaternion.identity);
                    carrot.transform.parent = this.transform;
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
