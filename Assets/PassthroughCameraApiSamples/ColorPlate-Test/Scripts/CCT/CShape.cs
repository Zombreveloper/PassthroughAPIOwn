using UnityEngine;
using UnityEngine.UIElements;

public class CShape : MonoBehaviour
{
    float radius;
    float innerRadius;

    int gapDirection;
    float gapWidth = 60f; // degrees

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateShape(float rad, int gapDir)
    {
        radius = rad;
        gapDirection = gapDir;
        innerRadius = radius * 0.6f;
    }

    public bool IsPositionInside(Vector2 circPos) //Hier kommen Daten über die Kreise der Platte als Argumente rein
    {
        float distance = circPos.magnitude;

        // Not in circle at all
        if (distance > radius) return false;

        // Calculate gap angle based on direction
        float gapAngle = gapDirection * 90f; // 0°, 90°, 180°, 270°
        float gapWidth = 60f; // degrees

        // Convert the plates circle position to angle relative to center of c
        float angle = Mathf.Atan2(circPos.y, circPos.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // Check if position is in the gap
        float angleDiff = Mathf.DeltaAngle(angle, gapAngle);
        bool isInGap = Mathf.Abs(angleDiff) < gapWidth / 2f;

        // Also check inner radius for C shape
        float innerRadius = radius * 0.6f;
        bool isInInnerCircle = distance < innerRadius;

        return !isInGap && !isInInnerCircle;
    }


}
