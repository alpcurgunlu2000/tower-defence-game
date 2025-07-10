using UnityEngine;
using System.Collections.Generic;

public class MazeSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject plotPrefab;
    public Transform parentContainer;

    [Header("Grid Settings")]
    public float spacingX = 1.2f;
    public float spacingY = 1.2f;

    [Header("Positioning")]
    public float verticalOffset = -0.3f;

    string[] maze = new string[]
    {
        "XXXXXXXXXXXXXX",
        "I XXX    XXXXX",
        "X XXX XX XXXXX",
        "X XXX XX XXXXX",
        "X XXX XX XX  O",
        "X XXX XX XX XX",
        "X     XX    XX",
        "XXXXXXXXXXXXXX"
    };

    private int rows;
    private int cols;
    private Vector3 offset;

    private Transform startPoint;
    private Transform endPoint;
    private List<Transform> pathPoints = new List<Transform>();

    [Header("Screen Layout")]
    [SerializeField] private float reservedTopPixels = 80f;

    private void Start()
    {
        rows = maze.Length;
        cols = maze[0].Length;

        float orthoHeight = Camera.main.orthographicSize * 2;
        float orthoWidth = Camera.main.aspect * orthoHeight;

        // Convert reserved pixels to world units
        float pixelsToWorld = orthoHeight / Screen.height;
        float reservedHeight = reservedTopPixels * pixelsToWorld;

        // Compute spacing
        float mazeHeight = orthoHeight - reservedHeight;
        spacingY = mazeHeight / rows;
        spacingX = orthoWidth / cols;

        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.z = 0;

        offset = cameraPos 
            + new Vector3(-orthoWidth / 2 + spacingX / 2, mazeHeight / 2 - spacingY / 2 - reservedHeight / 2, 0);

        GeneratePlots();
        GeneratePath();
        Debug.Log("MazeSpawner: Path generated, setting in LevelManager");
        LevelManager.main.SetPath(pathPoints.ToArray(), startPoint, endPoint);
    }

    private void GeneratePlots()
    {
        for (int y = 0; y < rows; y++)
        {
            string row = maze[y];
            for (int x = 0; x < row.Length; x++)
            {
                char c = row[x];
                Vector3 position = GetWorldPosition(x, y);

                if (c == 'X')
                {
                    GameObject newPlot = Instantiate(plotPrefab, position, Quaternion.identity, parentContainer);
                    newPlot.name = $"Plot ({x},{y})";
                }
            }
        }
    }

    private void GeneratePath()
    {
        bool[,] visited = new bool[rows, cols];
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();

        // Find start point 'I'
        for (int y = 0; y < rows; y++)
        {
            string row = maze[y];
            for (int x = 0; x < row.Length; x++)
            {
                if (row[x] == 'I')
                {
                    queue.Enqueue((x, y));
                    visited[y, x] = true;

                    GameObject startObj = new GameObject("Start Point");
                    startObj.transform.position = GetWorldPosition(x, y);
                    startPoint = startObj.transform;
                    pathPoints.Add(startPoint);
                    break;
                }
            }
            if (startPoint != null) break;
        }

        // BFS to trace path
        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();

            foreach (var (nx, ny) in GetNeighbors(cx, cy))
            {
                if (!visited[ny, nx] && (maze[ny][nx] == ' ' || maze[ny][nx] == 'O'))
                {
                    visited[ny, nx] = true;

                    GameObject pathObj = new GameObject($"Path Point ({nx},{ny})");
                    pathObj.transform.position = GetWorldPosition(nx, ny);
                    pathPoints.Add(pathObj.transform);

                    if (maze[ny][nx] == 'O')
                    {
                        endPoint = pathObj.transform;
                        return; // path done
                    }

                    queue.Enqueue((nx, ny));
                }
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * spacingX, -y * spacingY, 0) + offset;
    }

    private List<(int, int)> GetNeighbors(int x, int y)
    {
        List<(int, int)> neighbors = new List<(int, int)>();
        if (x > 0) neighbors.Add((x - 1, y));
        if (x < cols - 1) neighbors.Add((x + 1, y));
        if (y > 0) neighbors.Add((x, y - 1));
        if (y < rows - 1) neighbors.Add((x, y + 1));
        return neighbors;
    }
}