using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float damage = 10f;
    public float attackRange = 50f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            Debug.Log("Hit: " + hit.transform.name);

            BigEnemy bigEnemy = hit.transform.GetComponentInParent<BigEnemy>();
            Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
            if (bigEnemy != null)
            {
                bigEnemy.TakeDamage(damage);
                Debug.Log("BigEnemy damaged!");
            }
            else if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy damaged!");
            }
        }
    }
}