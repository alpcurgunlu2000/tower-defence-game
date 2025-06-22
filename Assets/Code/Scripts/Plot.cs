using UnityEngine;

public class Plot : MonoBehaviour
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

    private void OnMouseEnter() // Mouse enters PLOT
    {
        Debug.Log("Mouse enters " + name);
        sr.color = hoverColor; // Hover Color
    }

    private void OnMouseExit() // Mouse leaves PLOT
    {
        Debug.Log("Mouse leaves " + name);
        sr.color = startColor; // Hover Color
    }

    private void OnMouseDown() // MOUSE clicks on PLOT
    {
        if (tower != null) return;
        Debug.Log("Build Tower here: " + name);

        GameObject towerToBuild = BuildManager.main.GetSelectedTower();
        Instantiate(tower, transform.position, Quaternion.identity);
        /*
        GameObject towerToBuild = BuildManager.main.GetSelectedTower();
        Instantiate(towerToBuild, transform.position, Quaternion.identity);
        tower = towerToBuild;
        */

    }



}
