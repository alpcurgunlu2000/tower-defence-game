using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Plot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
[SerializeField] private PlotLabel plotLabel; // new reference
    private GameObject tower;
    private Color startColor;

    [Header("Attributes")]
    [SerializeField] private int towerCost = 20;

  private int index;
    public void SetIndex(int i)
    {
        index = i;
        if (plotLabel != null)
            plotLabel.SetLabel(index);
    }

    private void Start()
    {
        startColor = sr.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = hoverColor; // Hover Color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sr.color = startColor; // Hover Color
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tower != null) return;
        
        // Create the tower building action
        var towerAction = new DelayedMouseAction(() => {
            Debug.Log("Build Tower here: " + name);
            
            // Check if player has enough coins
            bool canBuild = FindObjectOfType<CoinManager>().SpendCoins(towerCost);
            if (canBuild)
            {
                GameObject towerToBuild = BuildManager.main.GetSelectedTower();
                Instantiate(towerToBuild, transform.position, Quaternion.identity);
                /*
                GameObject towerToBuild = BuildManager.main.GetSelectedTower();
                Instantiate(towerToBuild, transform.position, Quaternion.identity);
                tower = towerToBuild;
                */
            }
            else
            {
                Debug.Log("Not enough coins to build tower!");
            }
        }, $"Build tower at {name}", "tower_commands");
        
        // Delay or execute the action
        if (MouseActionDelayer.Instance != null)
        {
            MouseActionDelayer.Instance.DelayMouseAction(towerAction, "tower_commands");
        }
        else
        {
            // Fallback if delayer is not available
            towerAction.Execute();
        }
    }

    public void PutTower()
    {
        if (tower != null) return;
        Debug.Log("Build Tower here: " + name);

        // Check if player has enough coins
        bool canBuild = FindObjectOfType<CoinManager>().SpendCoins(towerCost);
        if (canBuild)
        {
            GameObject towerToBuild = BuildManager.main.GetSelectedTower();
            Instantiate(towerToBuild, transform.position, Quaternion.identity);
            /*
            GameObject towerToBuild = BuildManager.main.GetSelectedTower();
            Instantiate(towerToBuild, transform.position, Quaternion.identity);
            tower = towerToBuild;
            */
        }
        else
        {
            Debug.Log("Not enough coins to build tower!");
        }
    }
}