using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    public GameManager gameManager;
    private Collider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (myCollider == null || gameManager == null) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, myCollider.bounds.size, 0f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Obstacle"))
            {
                gameManager.FailStage();
                return;
            }

            if (hit.CompareTag("Goal"))
            {
                gameManager.ClearStage();
                return;
            }
        }
    }
}