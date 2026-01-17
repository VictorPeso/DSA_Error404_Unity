using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 2f;

    void Start()
    {
        // La bala se autodestruye a los 2 segundos para no llenar el juego de basura
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si choca con un Enemigo...
        if (collision.gameObject.CompareTag("Enemy")) // Asegúrate de que el enemigo tenga el Tag "Enemy"
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // ¡AQUÍ ESTABA EL ERROR! Ahora llamamos a la función correcta
                enemy.TakeDamage(damage);
            }
        }

        // La bala se destruye al chocar con cualquier cosa
        Destroy(gameObject);
    }
}