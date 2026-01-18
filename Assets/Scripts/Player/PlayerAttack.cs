using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración")]
    public float damage = 10f;
    public float range = 20f;

    [Header("Referencias")]
    public Transform firePoint;

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

        // Dirección del disparo = hacia donde mira el player (transform.forward)
        Vector3 direction = transform.forward;

        // Dibujar el rayo en la escena (rojo durante 2 segundos para verlo mejor)
        Debug.DrawRay(origin, direction * range, Color.red, 2f);
        Debug.Log($"[PlayerAttack] Disparando desde: {origin} hacia: {direction} (Player mirando: {transform.eulerAngles})");

        // Raycast para detectar si golpeamos algo
        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
        {
            Debug.Log($"[PlayerAttack] Raycast golpeó: {hit.collider.name} (Tag: '{hit.collider.tag}') a distancia: {hit.distance}m");

            // Verificar si lo que golpeamos es un enemigo
            // Primero buscar en el objeto golpeado
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            
            // Si no lo encuentra, buscar en el PADRE (por si golpeamos ojos, brazos, etc.)
            if (enemy == null)
            {
                enemy = hit.collider.GetComponentInParent<Enemy>();
            }
            
            if (enemy != null)
            {
                // Usar daño calculado del arma equipada (si existe EquipmentManager)
                float calculatedDamage = damage; // Valor por defecto
                
                if (EquipmentManager.Instance != null)
                {
                    calculatedDamage = EquipmentManager.Instance.totalDamage;
                }
                
                Debug.Log($"¡PUM! Golpeaste al enemigo: {enemy.name} - Daño: {calculatedDamage}");
                enemy.TakeDamage(calculatedDamage);
            }
            else
            {
                // Verificar si el tag es Enemy pero no tiene el componente
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.LogWarning($"¡ADVERTENCIA! {hit.collider.name} tiene el tag 'Enemy' pero NO tiene el componente Enemy.cs");
                }
                else
                {
                    Debug.Log($"Golpeaste algo pero no es un enemigo: {hit.collider.name} (Tag: '{hit.collider.tag}')");
                }
            }
        }
        else
        {
            Debug.Log("[PlayerAttack] Disparo al aire, no golpeó nada");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el rango de disparo en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * range);
    }
}
