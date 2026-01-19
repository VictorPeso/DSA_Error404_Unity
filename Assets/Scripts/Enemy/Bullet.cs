using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if (hitTransform.CompareTag("Player"))
        {
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(10f);
        }
        Destroy(gameObject);
    }
}
