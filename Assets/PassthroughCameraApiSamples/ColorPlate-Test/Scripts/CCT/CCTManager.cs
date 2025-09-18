/* So, das hier wird jetzt die Hauptklasse, die den CCTCreator möglichst gut in andere Klassen ausgelagert nachstellt
 * Derzeitiges Ziel: Testplate importieren und auf das Panel setzen.
 */

using UnityEngine;

public class CCTManager : MonoBehaviour
{
    [SerializeField] private GameObject cctPlate;
    [SerializeField] private RectTransform stimulusContainer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPlate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetPlate()
    {
        RectTransform containerRect = stimulusContainer.GetComponent<RectTransform>();
        var width = containerRect.rect.width;
        var height = containerRect.rect.height;

        //Mittelpunkt des panels finden
        float cx = width / 2;
        float cy = height / 2;
        Vector2 panelCenter = new Vector2(cx, cy);

        cctPlate.SetActive(true);
        var plateData = cctPlate.GetComponent<TestPlate>();
        plateData.InitializeValues();

        cctPlate.transform.SetParent(stimulusContainer);
        cctPlate.transform.localPosition = containerRect.position;
        cctPlate.transform.rotation = containerRect.rotation;

        //Skalierung der TestPlate
        // Panel-Größe in Weltkoordinaten
        Vector2 panelSize = stimulusContainer.rect.size * stimulusContainer.lossyScale;

        var objSize = plateData.BoundingBox.size;
        float scaleX = panelSize.x / objSize.x;
        float scaleY = panelSize.y / objSize.y;
        float finalScale = Mathf.Min(scaleX, scaleY);

        //cctPlate.InitializeValues();

    }
}
