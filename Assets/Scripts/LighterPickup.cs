using UnityEngine;

public class LighterPickup : MonoBehaviour
{
    public GameObject inventoryIcon;

    void OnMouseDown()
    {
        gameObject.SetActive(false);
        if (inventoryIcon != null) inventoryIcon.SetActive(true);
    }
}