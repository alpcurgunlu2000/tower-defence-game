using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private GameObject selectedTurretPrefab;
    private TurretButton activeButton;

    private void Awake()
    {
        main = this;
    }

    public GameObject GetSelectedTower()
    {
        return selectedTurretPrefab;
    }
    // Called by your TurretButton when clicked
    public void SelectTurret(GameObject turretPrefab)
    {
        selectedTurretPrefab = turretPrefab;
        Debug.Log($"Selected turret: {selectedTurretPrefab.name}");
    }

    // Called by your TurretButton to handle highlight visuals
    public void HighlightButton(TurretButton button)
    {
        if (activeButton != null)
            activeButton.Deselect();

        activeButton = button;
        activeButton.Select();
    }
}