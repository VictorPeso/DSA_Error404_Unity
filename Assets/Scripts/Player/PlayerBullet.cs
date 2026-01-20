using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}