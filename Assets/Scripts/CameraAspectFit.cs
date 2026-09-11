using UnityEngine;

public class CameraAspectFit : MonoBehaviour
{
    public float targetWidth = 20f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Fit();
    }

    void Fit()
    {
        if (cam == null || !cam.orthographic) return;
        cam.orthographicSize = targetWidth / cam.aspect / 2f;
    }

    void Update()
    {
        Fit();
    }
}