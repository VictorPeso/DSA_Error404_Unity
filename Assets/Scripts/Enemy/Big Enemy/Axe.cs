using UnityEngine;

public class Axe : MonoBehaviour
{
    public float damage = 60f; 
    public bool isAttacking = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                isAttacking = false; 
            }
        }
    }
}