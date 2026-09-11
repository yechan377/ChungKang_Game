using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CurveDrawer : MonoBehaviour
{
    public float minPointDistance = 0.1f;
    public int segmentsPerCurve = 10;

    public float maxInkLength = 15f;
    private float currentInkUsed = 0f;

    public Color unusedColor = new Color(0.35f, 0.55f, 0.95f);
    public Color traveledColor = Color.black;

    private bool connectedToPrevious = false;

    public bool IsConnectedToPrevious()
    {
        return connectedToPrevious;
    }

    public void SetConnectedToPrevious(bool value)
    {
        connectedToPrevious = value;
    }

    private bool isPortalJump = false;

    public bool IsPortalJump()
    {
        return isPortalJump;
    }

    public void SetPortalJump(bool value)
    {
        isPortalJump = value;
    }

    private LineRenderer lineRenderer;
    private List<Vector3> rawPoints = new List<Vector3>();
    private List<Vector3> smoothPoints = new List<Vector3>();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ApplyTraveledGradient(0f);
    }

    public bool TryAddPoint(Vector3 newPoint)
    {
        if (rawPoints.Count > 0)
        {
            float dist = Vector3.Distance(rawPoints[rawPoints.Count - 1], newPoint);
            if (dist < minPointDistance) return true;

            if (currentInkUsed + dist > maxInkLength)
            {
                return false;
            }

            currentInkUsed += dist;
        }

        rawPoints.Add(newPoint);
        RebuildSmoothCurve();
        return true;
    }

    public float GetRemainingInk()
    {
        return maxInkLength - currentInkUsed;
    }

    public bool HasInkRemaining()
    {
        return currentInkUsed < maxInkLength;
    }

    void RebuildSmoothCurve()
    {
        smoothPoints.Clear();

        if (rawPoints.Count < 2)
        {
            lineRenderer.positionCount = rawPoints.Count;
            lineRenderer.SetPositions(rawPoints.ToArray());
            return;
        }

        for (int i = 0; i < rawPoints.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? rawPoints[i] : rawPoints[i - 1];
            Vector3 p1 = rawPoints[i];
            Vector3 p2 = rawPoints[i + 1];
            Vector3 p3 = (i + 2 < rawPoints.Count) ? rawPoints[i + 2] : rawPoints[i + 1];

            for (int j = 0; j < segmentsPerCurve; j++)
            {
                float t = j / (float)segmentsPerCurve;
                smoothPoints.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }
        smoothPoints.Add(rawPoints[rawPoints.Count - 1]);

        lineRenderer.positionCount = smoothPoints.Count;
        lineRenderer.SetPositions(smoothPoints.ToArray());
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    public Vector3 GetLastRawPoint()
    {
        if (rawPoints.Count == 0) return Vector3.zero;
        return rawPoints[rawPoints.Count - 1];
    }

    public int GetPointCount()
    {
        return rawPoints.Count;
    }

    public List<Vector3> GetSmoothPoints()
    {
        return smoothPoints;
    }

    public void SetPredefinedPoints(List<Vector3> points)
    {
        rawPoints = new List<Vector3>(points);
        smoothPoints = new List<Vector3>(points);
        lineRenderer.positionCount = smoothPoints.Count;
        lineRenderer.SetPositions(smoothPoints.ToArray());
    }

    public bool IsPointNearPath(Vector3 point, float tolerance)
    {
        foreach (Vector3 p in smoothPoints)
        {
            if (Vector3.Distance(p, point) <= tolerance) return true;
        }
        return false;
    }

    public float GetTotalSmoothLength()
    {
        float length = 0f;
        for (int i = 0; i < smoothPoints.Count - 1; i++)
        {
            length += Vector3.Distance(smoothPoints[i], smoothPoints[i + 1]);
        }
        return length;
    }

    public void SetTraveledFraction(float fraction)
    {
        ApplyTraveledGradient(fraction);
    }

    void ApplyTraveledGradient(float fraction)
    {
        fraction = Mathf.Clamp01(fraction);
        Gradient gradient = new Gradient();

        if (fraction <= 0f)
        {
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(unusedColor, 0f), new GradientColorKey(unusedColor, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }
        else if (fraction >= 1f)
        {
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(traveledColor, 0f), new GradientColorKey(traveledColor, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }
        else
        {
            float edgeAfter = Mathf.Min(fraction + 0.001f, 1f);
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(traveledColor, 0f),
                    new GradientColorKey(traveledColor, fraction),
                    new GradientColorKey(unusedColor, edgeAfter),
                    new GradientColorKey(unusedColor, 1f)
                },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }

        lineRenderer.colorGradient = gradient;
    }
}