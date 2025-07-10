using UnityEngine;
using TMPro;

public class PlotLabel : MonoBehaviour
{
    private TextMeshPro tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    public void SetLabel(int number)
    {
        if (tmp == null)
            tmp = GetComponent<TextMeshPro>();

        tmp.text = $"{number}";
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(tmp);
#endif
    }
}