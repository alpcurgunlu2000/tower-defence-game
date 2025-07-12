using UnityEngine;
using UnityEngine.EventSystems;

public class ModalityManager : MonoBehaviour
{
    public static ModalityManager Instance { get; private set; }
    
    [Header("Modality Fission Settings")]
    [SerializeField] private float turretSelectionTimeout = 5f; // Timeout for turret selection commands
    
    private bool isTurretSelectionInProgress = false;
    private float turretSelectionStartTime;
    private bool mouseInteractionsDisabled = false;
    
    public bool IsMouseInteractionsDisabled => mouseInteractionsDisabled;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartTurretSelection()
    {
        isTurretSelectionInProgress = true;
        turretSelectionStartTime = Time.time;
        DisableMouseInteractions();
        Debug.Log("ModalityManager: Turret selection started - mouse interactions disabled");
    }
    
    public void CompleteTurretSelection()
    {
        isTurretSelectionInProgress = false;
        EnableMouseInteractions();
        Debug.Log("ModalityManager: Turret selection completed - mouse interactions enabled");
    }
    
    private void DisableMouseInteractions()
    {
        mouseInteractionsDisabled = true;
    }
    
    private void EnableMouseInteractions()
    {
        mouseInteractionsDisabled = false;
    }
    
    private void Update()
    {
        // Check for timeout on turret selection
        if (isTurretSelectionInProgress && Time.time - turretSelectionStartTime > turretSelectionTimeout)
        {
            Debug.Log("ModalityManager: Turret selection timeout - re-enabling mouse interactions");
            CompleteTurretSelection();
        }
    }
    
    public bool ShouldBlockMouseInteraction()
    {
        return mouseInteractionsDisabled;
    }
} 