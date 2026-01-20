using UnityEngine;

public class VisualProjectile : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
            rb.useGravity = false;
        }

        IgnorarColisionesConJugador();
    }

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    void IgnorarColisionesConJugador()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            Collider enemyCollider = enemy.GetComponent<Collider>();
            Collider projectileCollider = GetComponent<Collider>();

            if (enemyCollider != null && projectileCollider != null)
            {
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        if (rb == null)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
