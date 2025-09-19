using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private BoxCollider2D col;

    public Event OnHit;

    public void DisableCollider()
    {
        col.enabled = false;
    }

    public void EnableCollider ()
    {
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
