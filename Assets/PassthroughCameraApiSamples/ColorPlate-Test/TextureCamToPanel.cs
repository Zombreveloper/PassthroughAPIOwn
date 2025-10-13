using UnityEngine;

[ExecuteAlways]
public class MatchCameraToPanel : MonoBehaviour
{
    public RectTransform targetPanel;
    public Camera cam;
    public float zOffset = -10f; // Abstand zur UI, wenn nötig

    void Update()
    {
        if (!targetPanel || !cam) return;

        // 1️⃣ Panel Bounds im Welt-Raum berechnen
        Vector3[] corners = new Vector3[4];
        targetPanel.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        // 2️⃣ Mittelpunkt
        Vector3 center = (bottomLeft + topRight) * 0.5f;

        // 3️⃣ Kamera-Position anpassen
        cam.transform.position = new Vector3(center.x, center.y, center.z + zOffset);
        cam.transform.rotation = targetPanel.rotation;

        // 4️⃣ Orthographic Size (halbe Höhe)
        float panelHeight = Vector3.Distance(corners[1], corners[0]);
        cam.orthographicSize = panelHeight * 0.5f;

        // 5️⃣ Aspect Ratio (Breite/Höhe)
        float panelWidth = Vector3.Distance(corners[3], corners[0]);
        float aspect = panelWidth / panelHeight;
        cam.aspect = aspect;
    }
}
