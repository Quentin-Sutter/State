using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private BoxCollider2D collider2D;

    private void Reset()
    {
        collider2D = GetComponent<BoxCollider2D>();
        collider2D.isTrigger = true;
    }

    private void Awake()
    {
        collider2D ??= GetComponent<BoxCollider2D>();
    }

    public void DisableCollider()
    {
        if (collider2D != null)
        {
            collider2D.enabled = false;
        }
    }

    public void EnableCollider()
    {
        if (collider2D != null)
        {
            collider2D.enabled = true;
        }
    }
}
