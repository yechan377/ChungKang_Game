using UnityEngine;
using UnityEngine.EventSystems;

public class LighterDragIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public LayerMask paperLayer;
    public float revealRadius = 1.5f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalAnchoredPosition;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalAnchoredPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint);

        rectTransform.position = canvas.transform.TransformPoint(localPoint);

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, paperLayer))
        {
            HiddenReveal[] hiddenSpots = FindObjectsOfType<HiddenReveal>();
            foreach (HiddenReveal spot in hiddenSpots)
            {
                spot.CheckReveal(hit.point, revealRadius);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = originalAnchoredPosition;
        canvasGroup.blocksRaycasts = true;
    }
}