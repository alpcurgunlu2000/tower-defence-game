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

    public void DelayedSelectTurret(GameObject turretPrefab){
        if (MouseActionDelayer.Instance.isCurrentActionSpeech){
            Debug.Log($"DelayedSelectTurret: Current action is speech, executing immediately");
            SelectTurret(turretPrefab);
            return;
        }
        Debug.Log($"DelayedSelectTurret: {turretPrefab.name}");
        var turretAction = new DelayedMouseAction(() => {
            SelectTurret(turretPrefab);
        }, $"Select turret: {turretPrefab.name}", "turret_select_commands");
        
        if (MouseActionDelayer.Instance != null)
        {
            Debug.Log($"DelayedSelectTurret: Delaying turret selection for {turretPrefab.name}");
            MouseActionDelayer.Instance.DelayMouseAction(turretAction, "turret_select_commands");
        } else {
            Debug.Log($"DelayedSelectTurret: No delayer instance found, executing immediately");
            turretAction.Execute();
        }
    }

    public void DelayedHighlightFrame(GameObject frameObject){
        if (MouseActionDelayer.Instance.isCurrentActionSpeech){
            Debug.Log($"HighlightFrame: Current action is speech, executing immediately");
            HighlightFrame(frameObject);
            return;
        }
        var frameAction = new DelayedMouseAction(() => {
            HighlightFrame(frameObject);
        }, $"Highlight frame: {frameObject.name}", "turret_select_commands");
        
        if (MouseActionDelayer.Instance != null)
        {
            MouseActionDelayer.Instance.DelayMouseAction(frameAction, "turret_select_commands");
        } else {
            Debug.Log($"DelayedHighlightFrame: No delayer instance found, executing immediately");
            frameAction.Execute();
        }
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