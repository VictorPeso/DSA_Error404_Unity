using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private float trapDamage = 20f;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(trapDamage);
                Destroy(gameObject);
            }
        }
    }
}