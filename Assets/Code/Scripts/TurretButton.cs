using UnityEngine;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour
{
    [Header("References")]
    public GameObject turretPrefab; // The prefab this button selects

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color32(0x31, 0x89, 0xFF, 0xFF); // HEX 3189FF

    private Image frameImage;

    private void Awake()
    {
        // get the Image component on the parent frame (TurretFrameBlue)
        frameImage = GetComponentInParent<Image>();
    }

    private void Start()
    {
        Deselect();
    }

    public void OnClick()
    {
        BuildManager.main.SelectTurret(turretPrefab);
    }

    public void Select()
    {
        if (frameImage != null)
            frameImage.color = selectedColor;
    }

    public void Deselect()
    {
        if (frameImage != null)
            frameImage.color = normalColor;
    }
}