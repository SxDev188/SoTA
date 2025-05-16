using TMPro;
using UnityEngine;


/// <summary>
/// Author: Sixten
/// Ignore all the stupid comments or names :p
/// </summary>

public class FinderHelper : MonoBehaviour
{
    public static GameObject FindInactiveByTag(string tag)
    {
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allObjects)
        {
            if (go.CompareTag(tag))
                return go;
        }
        return null;
    }
}
