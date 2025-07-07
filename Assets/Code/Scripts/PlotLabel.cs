using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

[ExecuteAlways]
public class PlotLabel : MonoBehaviour
{
    void Awake()
    {
        UpdateLabel();
    }

    void OnValidate()
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        var tmp = GetComponent<TextMeshPro>();
        if (tmp == null)
        {
            Debug.LogWarning($"No TextMeshPro component found on {gameObject.name}");
            return;
        }

        string parentName = transform.parent != null ? transform.parent.name : "";

        string number = ExtractNumberFromName(parentName);

        // Only assign number if found, else empty string
        tmp.text = number != null ? number : "";
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(tmp);
#endif
    }

    private string ExtractNumberFromName(string name)
    {
        var match = Regex.Match(name, @"\((\d+)\)");
        if (match.Success)
            return match.Groups[1].Value;
        return null;
    }
}
