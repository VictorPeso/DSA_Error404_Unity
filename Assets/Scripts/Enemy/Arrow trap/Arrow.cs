using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 15f;
    public float lifeTime = 3f; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                float randomDamage = Random.Range(damage - 5, damage + 5);
                playerHealth.TakeDamage(randomDamage);
            }
            Destroy(gameObject); 
        }
        else if (other.CompareTag("Untagged") || other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
