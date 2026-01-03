using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Bullet collided with: " + collision.gameObject.name);
        Transform hitTransform = collision.transform;
        if (hitTransform.CompareTag("Player"))
        {
            Debug.Log("Bullet hit the player!");
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10f);
        }
        Destroy(gameObject);
    }
}
