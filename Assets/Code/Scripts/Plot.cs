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

        GameObject towerToBuild = BuildManager.main.GetSelectedTower();
        Instantiate(towerToBuild, transform.position, Quaternion.identity);
        /*
        GameObject towerToBuild = BuildManager.main.GetSelectedTower();
        Instantiate(towerToBuild, transform.position, Quaternion.identity);
        tower = towerToBuild;
        */

    }



}
