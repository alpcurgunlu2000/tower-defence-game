using UnityEngine;

public class PlotSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject plotPrefab; // assign your plot prefab in the inspector
    public Transform parentContainer; // an empty GameObject to keep hierarchy clean

    [Header("Grid Settings")]
    public int rows = 10;
    public int cols = 10;
    public float spacingX = 1.2f;
    public float spacingY = 1.2f;

    private int plotIndex = 0;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 position = new Vector3(x * spacingX, y * spacingY, 0);
                GameObject newPlot = Instantiate(plotPrefab, position, Quaternion.identity, parentContainer);
                newPlot.name = $"Plot ({plotIndex})";
                plotIndex++;
            }
        }
    }
}