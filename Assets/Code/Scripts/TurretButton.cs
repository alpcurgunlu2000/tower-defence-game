using UnityEngine;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour
{
    [Header("References")]
    public Image frameImage; // The background image to highlight
    public GameObject turretPrefab; // The prefab this button selects

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private void Start()
    {
        Deselect();
    }

    public void OnClick()
  {
      BuildManager.main.SelectTurret(turretPrefab);
      BuildManager.main.HighlightButton(this);
  }

    public void Select()
    {
        frameImage.color = selectedColor;
    }

    public void Deselect()
    {
        frameImage.color = normalColor;
    }
}