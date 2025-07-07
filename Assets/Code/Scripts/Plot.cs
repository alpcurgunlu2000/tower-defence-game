using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Plot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject tower;
    private Color startColor;

    [Header("Attributes")]
    [SerializeField] private int towerCost = 20;

    private void Start()
    {
        startColor = sr.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse enters " + name);
        sr.color = hoverColor; // Hover Color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse leaves " + name);
        sr.color = startColor; // Hover Color
    }

    public void OnPointerClick(PointerEventData eventData)
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