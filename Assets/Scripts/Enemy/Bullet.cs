using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Bullet collided with: " + collision.gameObject.name);
        // Add logic for what happens when the bullet collides with something
        Transform hitTransform = collision.transform;
        if (hitTransform.CompareTag("Player"))
        {
            Debug.Log("Bullet hit the player!");
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10f);
            // You can add damage logic here
        }
        Destroy(gameObject);
    }
}
