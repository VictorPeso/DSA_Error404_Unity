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

    // Ruta
    public Path path;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight = 1.6f;

    [Header("Weapon Values")]
    public Transform gunBarrel;
    public float fireRate = 1f;

    [SerializeField] private string currentState;

    [Header("Health System")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] EnemyHealth healthBar;

    [Header("Events")]
    [Tooltip("Evento que se dispara cuando el enemigo muere")]
    public UnityEvent OnDeath = new UnityEvent();

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        health = maxHealth;

        if (healthBar == null) healthBar = GetComponentInChildren<EnemyHealth>();

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        // Solo inicializar StateMachine si existe (opcional para bosses)
        if (stateMachine != null)
        {
            stateMachine.Initialize();
        }
        else
        {
            Debug.LogWarning($"[Enemy] '{gameObject.name}' no tiene StateMachine. Esto es normal para bosses.");
        }
    }

    void Update()
    {
        if (stateMachine != null && stateMachine.activeState != null)
            currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;


        if (Vector3.Distance(transform.position, player.transform.position) < sightDistance)
        {
            Vector3 origin = transform.position + (Vector3.up * eyeHeight) + (transform.forward * 0.8f);

            Vector3 target = player.transform.position + (Vector3.up * 0.5f);

            Vector3 direction = (target - origin).normalized;

            if (Vector3.Angle(transform.forward, direction) < fieldOfView)
            {
                if (Physics.Raycast(origin, direction, out RaycastHit hit, sightDistance))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.DrawLine(origin, hit.point, Color.red);
                        return true;
                    }
                    else
                    {
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