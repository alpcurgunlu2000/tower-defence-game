using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MouseActionDelayer : MonoBehaviour
{
    public static MouseActionDelayer Instance { get; private set; }
    
    [Header("Delayed Action Settings")]
    [SerializeField] private float maxDelayTime = 10f; // Maximum time to delay actions
    
    private bool isSpeechProcessing = false;
    private float speechStartTime;
    private Queue<DelayedMouseAction> delayedActions = new Queue<DelayedMouseAction>();
    
    public bool IsSpeechProcessing => isSpeechProcessing;
    public bool isCurrentActionSpeech = false;
    
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
    
    public void StartSpeechProcessing()
    {
        isSpeechProcessing = true;
        speechStartTime = Time.time;
        Debug.Log("MouseActionDelayer: Speech processing started - mouse actions will be delayed");
    }
    
    public void CompleteSpeechProcessing(string commandResult)
    {
        isSpeechProcessing = false;
        
        // Check if the command is conflicting (turret_select_commands)
        bool isPossiblyConflictingCommand = commandResult != null && commandResult.Contains("turret_select_commands");
        
        if (isPossiblyConflictingCommand)
        {
            Debug.Log("MouseActionDelayer: Possibly conflicting command detected - clearing turret selection actions");
            // Check is there any waiting "turret_select_commands" actions
            ClearConflictingActionsAndExecuteOthers("turret_select_commands");
        }
        else
        {
            Debug.Log("MouseActionDelayer: No conflicting command - executing delayed actions");
            ExecuteDelayedActions();
        }
    }
    
    public void DelayMouseAction(DelayedMouseAction action)
    {
        if (!isSpeechProcessing)
        {
            // No speech processing, execute immediately
            action.Execute();
            return;
        }
        
        // Check if we've exceeded max delay time
        if (Time.time - speechStartTime > maxDelayTime)
        {
            Debug.Log("MouseActionDelayer: Max delay time exceeded - executing action immediately");
            action.Execute();
            return;
        }
        
        // Add to delayed actions queue
        delayedActions.Enqueue(action);
        Debug.Log($"MouseActionDelayer: Action delayed - {action.Description}");
    }
    
    public void DelayMouseAction(DelayedMouseAction action, string actionType)
    {
        if (!isSpeechProcessing)
        {
            Debug.Log("MouseActionDelayer: No speech processing, executing action immediately");
            // No speech processing, execute immediately
            action.Execute();
            return;
        }
        
        // Check if we've exceeded max delay time
        if (Time.time - speechStartTime > maxDelayTime)
        {
            Debug.Log("MouseActionDelayer: Max delay time exceeded - executing action immediately");
            action.Execute();
            return;
        }
        
        // Add action type to the delayed action
        action.ActionType = actionType;
        
        // Add to delayed actions queue
        delayedActions.Enqueue(action);
        Debug.Log($"MouseActionDelayer: Action delayed - {action.Description} (Type: {actionType})");
    }
    
    private void ExecuteDelayedActions()
    {
        int actionCount = delayedActions.Count;
        while (delayedActions.Count > 0)
        {
            var action = delayedActions.Dequeue();
            action.Execute();
        }
        
        if (actionCount > 0)
        {
            Debug.Log($"MouseActionDelayer: Executed {actionCount} delayed actions");
        }
    }
    
    private void ClearDelayedActions()
    {
        int actionCount = delayedActions.Count;
        delayedActions.Clear();
        
        if (actionCount > 0)
        {
            Debug.Log($"MouseActionDelayer: Cleared {actionCount} delayed actions due to conflicting command");
        }
    }
    
    private void ClearConflictingActionsAndExecuteOthers(string conflictingActionType)
    {
        var remainingActions = new Queue<DelayedMouseAction>();
        int clearedCount = 0;
        
        while (delayedActions.Count > 0)
        {
            var action = delayedActions.Dequeue();
            if (action.ActionType == conflictingActionType)
            {
                clearedCount++;
                Debug.Log($"MouseActionDelayer: Cleared conflicting action - {action.Description}");
            }
            else
            {
                // execute the action
                action.Execute();
            }
        }
        
        // Restore non-conflicting actions
        delayedActions = remainingActions;
        
        if (clearedCount > 0)
        {
            Debug.Log($"MouseActionDelayer: Cleared {clearedCount} conflicting actions, kept {delayedActions.Count} non-conflicting actions");
        }
    }
    
    private void Update()
    {
        // Check for timeout on speech processing
        if (isSpeechProcessing && Time.time - speechStartTime > maxDelayTime)
        {
            Debug.Log("MouseActionDelayer: Speech processing timeout - executing delayed actions");
            isSpeechProcessing = false;
            ExecuteDelayedActions();
        }
    }

    public void SetCurrentActionSpeech(bool isCurrentActionSpeech)
    {
        this.isCurrentActionSpeech = isCurrentActionSpeech;
    }
}

// Class to represent a delayed mouse action
public class DelayedMouseAction
{
    public System.Action ExecuteAction { get; private set; }
    public string Description { get; private set; }
    public string ActionType { get; set; } = "";
    
    public DelayedMouseAction(System.Action action, string description)
    {
        ExecuteAction = action;
        Description = description;
    }
    
    public DelayedMouseAction(System.Action action, string description, string actionType)
    {
        ExecuteAction = action;
        Description = description;
        ActionType = actionType;
    }
    
    public void Execute()
    {
        ExecuteAction?.Invoke();
    }
} 