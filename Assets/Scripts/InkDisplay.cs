using UnityEngine;
using UnityEngine.UI;

public class InkDisplay : MonoBehaviour
{
    public PathManager pathManager;
    private Image fillImage;
    private float maxInk = -1f;

    void Awake()
    {
        fillImage = GetComponent<Image>();
    }

    public void Initialize()
    {
        if (pathManager == null || fillImage == null) return;

        maxInk = pathManager.GetRemainingInk();
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        if (pathManager == null || fillImage == null || maxInk <= 0f) return;

        float remaining = pathManager.GetRemainingInk();
        fillImage.fillAmount = Mathf.Clamp01(remaining / maxInk);
    }
}