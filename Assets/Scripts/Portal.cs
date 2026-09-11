using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    private bool used = false;

    public bool IsUsed()
    {
        return used;
    }

    public void MarkUsed()
    {
        used = true;
        if (linkedPortal != null) linkedPortal.used = true;
    }
}