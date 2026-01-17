using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Bullet collided with: " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemyComponent))
        {
            Debug.Log("Bullet hit the enemy!");
            collision.gameObject.GetComponent<Enemy>().TakeDamage(10f);
        }
        if (collision.gameObject.TryGetComponent<BigEnemy>(out BigEnemy BigEnemyComponent))
        {
            Debug.Log("Bullet hit the big enemy!");
            collision.gameObject.GetComponent<BigEnemy>().TakeDamage(10f);
        }
            Destroy(gameObject);
    }
}
