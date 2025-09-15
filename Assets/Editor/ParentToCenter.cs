using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParentToCenter : MonoBehaviour
{
    [MenuItem("Tools/Center Parent On Children")]
    private static void CenterParent()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Bitte ein GameObject auswählen.");
            return;
        }

        GameObject parent = Selection.activeGameObject;
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Keine Renderer in den Childs gefunden.");
            return;
        }

        // Bounds der Child-Renderer zusammenfassen
        Bounds bounds = renderers[0].bounds;
        foreach (var rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        Vector3 center = bounds.center;

        // Kinder temporär abtrennen
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child);
            child.SetParent(null, true);
        }

        // Parent verschieben
        Undo.RecordObject(parent.transform, "Center Parent");
        parent.transform.position = center;

        // Kinder wieder anhängen
        foreach (Transform child in children)
        {
            child.SetParent(parent.transform, true);
        }

        Debug.Log($"Parent '{parent.name}' wurde ins Zentrum der Childs verschoben.");
    }
}
