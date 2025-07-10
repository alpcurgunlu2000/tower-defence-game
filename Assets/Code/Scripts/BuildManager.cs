using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color32(0x31, 0x89, 0xFF, 0xFF);

    [Header("Turret Buttons")]
    public GameObject redTurretButtonObj;
    public GameObject greenTurretButtonObj;
    public GameObject blueTurretButtonObj;

    private GameObject selectedTurretPrefab;
    private GameObject activeFrame;

    private void Awake()
    {
        main = this;
    }

    public GameObject GetSelectedTower()
    {
        return selectedTurretPrefab;
    }

    public void SelectTurret(GameObject turretPrefab)
    {
        selectedTurretPrefab = turretPrefab;
        Debug.Log($"Selected turret: {selectedTurretPrefab.name}");
    }

    public void HighlightFrame(GameObject frameObject)
    {
        if (activeFrame != null && activeFrame != frameObject)
        {
            SetFrameColor(activeFrame, normalColor);
        }

        activeFrame = frameObject;
        SetFrameColor(activeFrame, selectedColor);
    }

    private void SetFrameColor(GameObject frame, Color color)
    {
        var image = frame.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }
    }
}