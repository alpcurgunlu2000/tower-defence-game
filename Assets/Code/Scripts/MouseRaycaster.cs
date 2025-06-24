using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Mouse ray hits: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Mouse ray hits nothing.");
            }
        }
    }
}
