using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración")]
    public float damage = 10f;
    public float range = 20f;

    [Header("Referencias")]
    public Transform firePoint;
    public GameObject visualProjectilePrefab;

    private InputManager inputManager;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        if (inputManager == null)
        {
            Debug.LogError("[PlayerAttack] InputManager no encontrado en el GameObject!");
        }
    }

    void Update()
    {
        // Usar InputManager para detectar disparo
        if (inputManager != null && inputManager.OnFoot.Fire.triggered)
        {
            Atacar();
        }
    }

    /// <summary>
    /// Dispara hacia donde está mirando el Player (dirección forward)
    /// NO busca enemigos automáticamente, dispara directo
    /// </summary>
    public void Atacar()
    {
        // Punto de origen del disparo (a media altura, centro del cuerpo)
        Vector3 origin = transform.position + Vector3.up * 0.5f + transform.forward * 0.5f;
        if (firePoint != null)
        {
            origin = firePoint.position;
        }

        Vector3 direction = transform.forward;

        // Instanciar proyectil visual
        if (visualProjectilePrefab != null)
        {
            GameObject projectile = Instantiate(visualProjectilePrefab, origin, Quaternion.LookRotation(direction));
            Debug.Log($"[PlayerAttack] Proyectil instanciado en {origin}");
        }
        else
        {
            Debug.LogWarning("[PlayerAttack] visualProjectilePrefab es null, asígnalo en el inspector");
        }

        Debug.DrawRay(origin, direction * range, Color.red, 2f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            if (enemy == null)
            {
                enemy = hit.collider.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                float calculatedDamage = damage;

                if (EquipmentManager.Instance != null)
                {
                    calculatedDamage = EquipmentManager.Instance.totalDamage;
                }

                enemy.TakeDamage(calculatedDamage);
            }
            else
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.LogWarning($"[PlayerAttack] {hit.collider.name} tiene el tag 'Enemy' pero NO tiene el componente Enemy.cs");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * range);
    }
}
