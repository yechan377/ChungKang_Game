using System.Collections.Generic;
using UnityEngine;

public class HiddenReveal : MonoBehaviour
{
    private LineRenderer lr;
    private List<Vector3> samplePoints = new List<Vector3>();
    private float[] revealedAlpha;
    private const int sampleCount = 8;

    public Color revealColor = Color.yellow;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        if (lr == null || lr.positionCount == 0) return;

        BuildSamplePoints();
        revealedAlpha = new float[samplePoints.Count];
        ApplyGradient();
    }

    Vector3 GetWorldPoint(int i)
    {
        return lr.useWorldSpace ? lr.GetPosition(i) : transform.TransformPoint(lr.GetPosition(i));
    }

    void BuildSamplePoints()
    {
        int n = lr.positionCount;

        if (n <= sampleCount)
        {
            for (int i = 0; i < n; i++) samplePoints.Add(GetWorldPoint(i));
        }
        else
        {
            for (int s = 0; s < sampleCount; s++)
            {
                int idx = Mathf.RoundToInt(s * (n - 1) / (float)(sampleCount - 1));
                samplePoints.Add(GetWorldPoint(idx));
            }
        }
    }

    public void CheckReveal(Vector3 worldPos, float radius)
    {
        if (lr == null || samplePoints.Count == 0) return;

        bool changed = false;

        for (int i = 0; i < samplePoints.Count; i++)
        {
            float dist = Vector3.Distance(samplePoints[i], worldPos);
            if (dist <= radius)
            {
                float alpha = 1f - (dist / radius);
                if (alpha > revealedAlpha[i])
                {
                    revealedAlpha[i] = alpha;
                    changed = true;
                }
            }
        }

        if (changed) ApplyGradient();
    }

    void ApplyGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(revealColor, 0f),
            new GradientColorKey(revealColor, 1f)
        };

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[samplePoints.Count];
        for (int i = 0; i < samplePoints.Count; i++)
        {
            float t = samplePoints.Count > 1 ? i / (float)(samplePoints.Count - 1) : 0f;
            alphaKeys[i] = new GradientAlphaKey(revealedAlpha[i], t);
        }

        gradient.SetKeys(colorKeys, alphaKeys);
        lr.colorGradient = gradient;
    }

    public bool IsRevealedAndNear(Vector3 point, float tolerance)
    {
        if (samplePoints.Count == 0) return false;

        for (int i = 0; i < samplePoints.Count; i++)
        {
            if (revealedAlpha[i] > 0.1f && Vector3.Distance(samplePoints[i], point) <= tolerance)
            {
                return true;
            }
        }
        return false;
    }

    public List<Vector3> GetLinePoints()
    {
        List<Vector3> points = new List<Vector3>();
        if (lr == null) return points;

        for (int i = 0; i < lr.positionCount; i++)
        {
            points.Add(GetWorldPoint(i));
        }
        return points;
    }
}