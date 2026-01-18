using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;

    [Header("Referencias UI")]
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header("Sistema de Respawn")]
    private Vector3 spawnPosition; // Para recordar dónde naciste
    private Quaternion spawnRotation;
    private CharacterController charController; // Por si usas CharacterController

    void Start()
    {
        // 1. Inicializar vida
        health = maxHealth;

        // 2. Guardar posición de nacimiento (Spawn)
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        // 3. Detectar si usamos CharacterController (necesario para el TP)
        charController = GetComponent<CharacterController>();

        // 4. Inicializar Barras
        if (frontHealthBar != null) frontHealthBar.fillAmount = 1f;
        if (backHealthBar != null) backHealthBar.fillAmount = 1f;
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        // Protección: Si no has asignado las imágenes en el Inspector, no hacemos nada para evitar errores
        if (frontHealthBar == null || backHealthBar == null) return;

        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;

        // Lógica de animación de la barra (Tu código original)
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        else if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;

        // --- AQUÍ ESTÁ EL CAMBIO ---
        // Si la vida llega a 0, ejecutamos el Respawn
        if (health <= 0)
        {
            Respawn();
        }
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    void Respawn()
    {
        if (charController != null) charController.enabled = false;

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        if (charController != null) charController.enabled = true;

        // 4. Resetear Vida y Barras visuales AL INSTANTE
        health = maxHealth;
        lerpTimer = 0f;

        if (frontHealthBar != null) frontHealthBar.fillAmount = 1f;
        if (backHealthBar != null) backHealthBar.fillAmount = 1f;
    }
}