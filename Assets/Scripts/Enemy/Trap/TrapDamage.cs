using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private float trapDamage = 20f;
   
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo entró en la trampa: " + other.name + " con Tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(trapDamage);
                Debug.Log("¡El jugador ha pisado una trampa!");

                Destroy(gameObject);
            }
        }
    }
}