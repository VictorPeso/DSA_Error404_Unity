using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Bullet collided with: " + collision.gameObject.name);
        // Add logic for what happens when the bullet collides with something
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemyComponent))
        {
            Debug.Log("Bullet hit the enemy!");
            collision.gameObject.GetComponent<Enemy>().TakeDamage(10f);
            // You can add damage logic here
        }
        Destroy(gameObject);
    }
}
