using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public float amplitude = 10f; // vertical movement
    public float frequency = 1f;  // speed

    private RectTransform rectTransform;
    private Vector2 startPos;

  void Start()
  {
      rectTransform = GetComponent<RectTransform>();
      startPos = rectTransform.anchoredPosition;
  }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;
        rectTransform.anchoredPosition = startPos + new Vector2(0, offset);
    }
}
