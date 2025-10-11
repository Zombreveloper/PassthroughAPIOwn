using UnityEngine;
using System;

public static class ChromaticityGamut
{
    // 2D Kreuzprodukt (skalar) für (x1,y1) x (x2,y2)
    private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    // Schnitt Ray(o, dir) mit Segment(a, b).
    // Gibt (bool hit, float t, float u)
    // - hit = true wenn Schnitt mit t >= 0 und u in [0,1]
    // - t = Parameter auf Ray (R(t) = o + t*dir)
    // - u = Parameter auf Segment (S(u) = a + u*(b-a))
    private static (bool hit, float t, float u) RaySegmentIntersection(Vector2 o, Vector2 dir, Vector2 a, Vector2 b, float eps = 1e-9f)
    {
        Vector2 r = dir;
        Vector2 s = b - a;
        Vector2 q_p = a - o; // q - p in Standardnotation (p=o, q=a)

        float rxs = Cross(r, s);
        if (Mathf.Abs(rxs) < eps)
        {
            // Parallel (oder beinahe). Kein eindeutiger Schnitt (oder kollinear).
            return (false, 0f, 0f);
        }

        float t = Cross(q_p, s) / rxs;   // t auf Ray
        float u = Cross(q_p, r) / rxs;   // u auf Segment

        // Akzeptiere nur t >= 0 (vorwärts auf dem Ray) und u in [0,1]
        if (t >= -eps && u >= -eps && u <= 1f + eps)
            return (true, t, u);
        else
            return (false, t, u);
    }

    /// <summary>
    /// Berechnet die maximale positive Distanz t (in Einheiten der Norm von dir) 
    /// sodass o + t*dir noch im Dreieck (p0,p1,p2) liegt.
    /// Wenn dir nicht normalisiert ist, entspricht t der Multiplikation auf dem unnormalisierten dir.
    /// </summary>
    /// <param name="o">Origin (xy)</param>
    /// <param name="dir">Richtungsvektor (xy). Empfehlung: normalize vor Aufruf.</param>
    /// <param name="p0">Primary R (xy)</param>
    /// <param name="p1">Primary G (xy)</param>
    /// <param name="p2">Primary B (xy)</param>
    /// <returns>maximaler t >= 0. Wenn kein Schnitt gefunden (theoretisch nicht möglich, wenn o im Dreieck), gibt float.PositiveInfinity zurück.</returns>
    public static float MaxDistanceInsideTriangle(Vector2 o, Vector2 dir, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        // Normiere die Richtung, damit t die "Länge" entlang dir ist.
        Vector2 d = dir.normalized; //Sorry Bro, ich will doch nicht normalisieren
        float eps = 1e-9f;

        // Liste der Kanten
        var edges = new (Vector2 a, Vector2 b)[] {
            (p0, p1),
            (p1, p2),
            (p2, p0)
        };

        float minPositiveT = float.PositiveInfinity;
        bool found = false;

        foreach (var edge in edges)
        {
            var res = RaySegmentIntersection(o, d, edge.a, edge.b, eps);
            if (res.hit)
            {
                // t ist die Entfernung entlang d (da d normalisiert)
                if (res.t >= 0f)
                {
                    found = true;
                    if (res.t < minPositiveT) minPositiveT = res.t;
                }
            }
        }

        if (!found)
        {
            // Theoretisch: o ist außerhalb oder numerische Probleme.
            return float.PositiveInfinity;
        }

        return minPositiveT;
    }
}

