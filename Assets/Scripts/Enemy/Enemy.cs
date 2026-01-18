using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;

    // Getters
    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }

    // Ruta (Opcional ahora)
    public Path path;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight = 1.6f; // Ajustado a altura humana

    [Header("Weapon Values")]
    public Transform gunBarrel;
    public float fireRate = 1f;

    [SerializeField] private string currentState;

    [Header("Health System")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 50f; // Valor por defecto
    [SerializeField] EnemyHealth healthBar; // Asigna el script EnemyHealth

    [Header("Events")]
    [Tooltip("Evento que se dispara cuando el enemigo muere (usado por LevelManager para detectar victoria)")]
    public UnityEvent OnDeath = new UnityEvent();

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Configurar Vida Inicial
        health = maxHealth;

        // PROTECCIÓN: Buscamos la barra de vida, pero si no está, no damos error
        if (healthBar == null) healthBar = GetComponentInChildren<EnemyHealth>();

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        // Arrancamos la máquina
        stateMachine.Initialize();
    }

    void Update()
    {
        // Solo para ver en el inspector qué hace
        if (stateMachine.activeState != null)
            currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        // 1. Distancia: ¿Está lo bastante cerca?
        if (Vector3.Distance(transform.position, player.transform.position) < sightDistance)
        {
            // --- CORRECCIÓN DE OJOS ---
            // Sacamos los ojos 0.8 metros hacia adelante para no chocarnos con nuestro propio cuerpo
            Vector3 origin = transform.position + (Vector3.up * eyeHeight) + (transform.forward * 0.8f);

            // Miramos al CENTRO del jugador (subimos 0.5m), no a sus pies
            Vector3 target = player.transform.position + (Vector3.up * 0.5f);

            Vector3 direction = (target - origin).normalized;

            // 2. Ángulo: ¿Está delante de mí?
            if (Vector3.Angle(transform.forward, direction) < fieldOfView)
            {
                // 3. Raycast: ¿Hay paredes en medio?
                if (Physics.Raycast(origin, direction, out RaycastHit hit, sightDistance))
                {
                    // Si el rayo toca al jugador...
                    if (hit.collider.CompareTag("Player"))
                    {
                        // DIBUJA LÍNEA ROJA (SI TE VE)
                        Debug.DrawLine(origin, hit.point, Color.red);
                        return true;
                    }
                    else
                    {
                        // DIBUJA LÍNEA AMARILLA (SI MIRA PERO ALGO LO TAPA)
                        // Esto te ayudará a ver si se está chocando con una pared
                        Debug.DrawLine(origin, hit.point, Color.yellow);
                    }
                }
            }
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // PROTECCIÓN: Solo actualizamos la barra si existe
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject, 0.1f);
    }
}