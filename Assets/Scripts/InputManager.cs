using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PathManager pathManager;
    public GameManager gameManager;
    public LayerMask paperLayer;

    public float resumeTolerance = 0.3f;
    public float overlapTolerance = 0.3f;

    private bool _canDraw = false;

    void Update()
    {
        if (gameManager == null || gameManager.startPoint == null || gameManager.goalPoint == null) return;
        if (gameManager.GetState() != GameManager.GameState.Drawing) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }

        if (Input.GetMouseButton(0) && _canDraw)
        {
            HandleMouseHeld();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _canDraw = false;
        }
    }

    void HandleMouseDown()
    {
        if (!TryGetPaperHitPoint(out Vector3 hitPoint))
        {
            _canDraw = false;
            return;
        }

        if (!pathManager.HasAnySegment())
        {
            if (!gameManager.IsNearStart(hitPoint))
            {
                _canDraw = false;
                return;
            }

            pathManager.StartNewSegment(hitPoint);
            _canDraw = true;
            return;
        }

        Vector3 lastPoint = pathManager.GetLastRawPoint();
        float dist = Vector3.Distance(lastPoint, hitPoint);

        if (dist <= resumeTolerance)
        {
            pathManager.TryAddPoint(lastPoint);
            _canDraw = true;
        }
        else
        {
            if (pathManager.CanStartNewSegment())
            {
                HiddenReveal touchedHidden = FindTouchedRevealedHiddenPath(hitPoint, overlapTolerance);
                Portal touchedPortal = FindTouchedPortal(hitPoint, overlapTolerance);

                bool touchesExisting = pathManager.IsTouchingAnySegment(hitPoint, overlapTolerance)
                    || touchedHidden != null
                    || touchedPortal != null;

                if (touchedHidden != null)
                {
                    pathManager.InsertPredefinedSegment(touchedHidden.GetLinePoints(), true);
                }

                if (touchedPortal != null)
                {
                    touchedPortal.MarkUsed();
                }

                pathManager.StartNewSegment(hitPoint, touchesExisting, touchedPortal != null);
                _canDraw = true;
            }
            else
            {
                _canDraw = false;
            }
        }
    }

    void HandleMouseHeld()
    {
        if (TryGetPaperHitPoint(out Vector3 hitPoint))
        {
            pathManager.TryAddPoint(hitPoint);
        }
    }

    bool TryGetPaperHitPoint(out Vector3 hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, paperLayer))
        {
            hitPoint = hit.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

    HiddenReveal FindTouchedRevealedHiddenPath(Vector3 point, float tolerance)
    {
        HiddenReveal[] hiddenSpots = FindObjectsOfType<HiddenReveal>();
        foreach (HiddenReveal spot in hiddenSpots)
        {
            if (spot.IsRevealedAndNear(point, tolerance)) return spot;
        }
        return null;
    }

    Portal FindTouchedPortal(Vector3 point, float tolerance)
    {
        Portal[] portals = FindObjectsOfType<Portal>();
        foreach (Portal portal in portals)
        {
            if (portal.IsUsed()) continue;
            if (Vector3.Distance(portal.transform.position, point) <= tolerance) return portal;
        }
        return null;
    }

    public float GetRemainingInk()
    {
        return pathManager.GetRemainingInk();
    }

    public void ResetDrawing()
    {
        if (gameManager.GetState() != GameManager.GameState.Drawing) return;

        pathManager.ClearAll();
        _canDraw = false;
    }
}