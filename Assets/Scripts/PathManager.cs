using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public GameObject curveSegmentPrefab;
    public Transform segmentParent;

    private List<CurveDrawer> segments = new List<CurveDrawer>();
    private CurveDrawer activeSegment;

    public void InsertPredefinedSegment(List<Vector3> points, bool connectedToPrevious)
    {
        float budgetForThisSegment = (activeSegment != null)
            ? activeSegment.GetRemainingInk()
            : curveSegmentPrefab.GetComponent<CurveDrawer>().maxInkLength;

        GameObject obj = Instantiate(curveSegmentPrefab, segmentParent);
        CurveDrawer newSegment = obj.GetComponent<CurveDrawer>();
        newSegment.maxInkLength = budgetForThisSegment;
        newSegment.SetPredefinedPoints(points);
        newSegment.SetConnectedToPrevious(connectedToPrevious);

        segments.Add(newSegment);
        activeSegment = newSegment;
    }

    public bool CanStartNewSegment()
    {
        if (activeSegment == null) return true;
        return activeSegment.HasInkRemaining();
    }

    public void StartNewSegment(Vector3 startPoint, bool connectedToPrevious = false, bool isPortalJump = false)
    {
        float budgetForThisSegment = (activeSegment != null)
            ? activeSegment.GetRemainingInk()
            : curveSegmentPrefab.GetComponent<CurveDrawer>().maxInkLength;

        GameObject obj = Instantiate(curveSegmentPrefab, segmentParent);
        CurveDrawer newSegment = obj.GetComponent<CurveDrawer>();
        newSegment.maxInkLength = budgetForThisSegment;
        newSegment.SetConnectedToPrevious(connectedToPrevious);
        newSegment.SetPortalJump(isPortalJump);

        newSegment.TryAddPoint(startPoint);

        segments.Add(newSegment);
        activeSegment = newSegment;
    }

    public bool TryAddPoint(Vector3 point)
    {
        if (activeSegment == null) return false;
        return activeSegment.TryAddPoint(point);
    }

    public Vector3 GetLastRawPoint()
    {
        if (activeSegment == null) return Vector3.zero;
        return activeSegment.GetLastRawPoint();
    }

    public bool HasAnySegment()
    {
        return activeSegment != null && activeSegment.GetPointCount() > 0;
    }

    public void ClearAll()
    {
        foreach (CurveDrawer segment in segments)
        {
            if (segment != null) Destroy(segment.gameObject);
        }
        segments.Clear();
        activeSegment = null;
    }

    public float GetRemainingInk()
    {
        return activeSegment != null
            ? activeSegment.GetRemainingInk()
            : curveSegmentPrefab.GetComponent<CurveDrawer>().maxInkLength;
    }

    public bool IsTouchingAnySegment(Vector3 point, float tolerance)
    {
        foreach (CurveDrawer segment in segments)
        {
            if (segment.IsPointNearPath(point, tolerance)) return true;
        }
        return false;
    }

    public List<CurveDrawer> GetSegmentsInOrder()
    {
        return new List<CurveDrawer>(segments);
    }
}