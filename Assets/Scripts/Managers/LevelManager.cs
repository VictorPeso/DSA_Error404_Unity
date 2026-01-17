using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor del nivel actual
/// - Teleporta jugador a SpawnRoom al inicio
/// - Detecta muerte del boss
/// - Actualiza progreso cuando boss muere
/// - Muestra panel de victoria
/// 
/// CONFIGURACIÓN EN INSPECTOR:
/// - Asignar levelNumber (1-5)
/// - Asignar levelName (ej: "Sector C:/")
/// - Arrastrar GameObject con tag "Respawn" a spawnRoom
/// - Arrastrar GameObject del boss (con Enemy.cs)
/// - Arrastrar victoryPanel (Canvas con VictoryPanel.cs)
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    [Tooltip("Número del nivel actual (1-5)")]
    public int levelNumber = 1;

    [Tooltip("Nombre descriptivo del nivel")]
    public string levelName = "Sector C:/";

    [Header("Referencias de Salas")]
    [Tooltip("GameObject con tag 'Respawn' - Punto de spawn del jugador")]
    public GameObject spawnRoom;

    [Tooltip("GameObject del Boss (debe tener Enemy.cs)")]
    public GameObject boss;

    [Header("UI")]
    [Tooltip("Panel de victoria (GameObject con VictoryPanel.cs) - DEBE estar oculto al inicio")]
    public GameObject victoryPanel;

    [Header("Sistema de Puntuación")]
    [Tooltip("Puntos por matar enemigo normal")]
    public int pointsPerEnemy = 10;

    [Tooltip("Puntos por derrotar boss")]
    public int pointsPerBoss = 500;

    [Tooltip("Bonus por completar nivel")]
    public int levelCompletionBonus = 200;

    private bool bossDefeated = false;

    void Start()
    {
        Debug.Log($"[LevelManager] Iniciando Nivel {levelNumber} - {levelName}");

        // 1. Buscar referencias si no están asignadas
        FindReferencesIfNeeded();

        // 2. Resetear score de partida
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetCurrentScore();
        }

        // 3. Teleportar jugador a spawn
        TeleportPlayerToSpawn();

        // 4. Suscribir evento de muerte del boss
        SubscribeToBossDeathEvent();
    }

    /// <summary>
    /// Busca referencias automáticamente si no están asignadas en el Inspector
    /// </summary>
    void FindReferencesIfNeeded()
    {
        // Buscar SpawnRoom por tag
        if (spawnRoom == null)
        {
            spawnRoom = GameObject.FindWithTag("Respawn");
            if (spawnRoom != null)
            {
                Debug.Log($"[LevelManager] SpawnRoom encontrado: {spawnRoom.name}");
            }
            else
            {
                Debug.LogWarning("[LevelManager] ⚠️ No se encontró GameObject con tag 'Respawn'");
                Debug.LogWarning("[LevelManager] El jugador no será teleportado al inicio");
            }
        }

        // Buscar Boss por tag
        if (boss == null)
        {
            boss = GameObject.FindWithTag("Boss");
            if (boss != null)
            {
                Debug.Log($"[LevelManager] Boss encontrado: {boss.name}");
            }
            else
            {
                Debug.LogWarning("[LevelManager] ⚠️ No se encontró GameObject con tag 'Boss'");
                Debug.LogWarning("[LevelManager] El sistema de victoria no funcionará");
            }
        }
    }

    /// <summary>
    /// Teleporta al jugador al punto de spawn
    /// También actualiza el spawn point de PlayerHealth para respawn
    /// </summary>
    void TeleportPlayerToSpawn()
    {
        if (spawnRoom == null)
        {
            Debug.LogWarning("[LevelManager] No se puede teleportar: spawnRoom es null");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[LevelManager] ❌ No se encontró GameObject con tag 'Player'");
            return;
        }

        // Desactivar CharacterController temporalmente para permitir teleport
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Teleportar
        player.transform.position = spawnRoom.transform.position;
        player.transform.rotation = spawnRoom.transform.rotation;

        // Reactivar CharacterController
        if (cc != null) cc.enabled = true;

        Debug.Log($"[LevelManager] Jugador teleportado a SpawnRoom: {spawnRoom.transform.position}");

        // PlayerHealth.Start() ya guarda el spawn point automáticamente
        // Como acabamos de mover al jugador, PlayerHealth usará esta nueva posición
    }

    /// <summary>
    /// Suscribe al evento de muerte del boss
    /// </summary>
    void SubscribeToBossDeathEvent()
    {
        if (boss == null)
        {
            Debug.LogWarning("[LevelManager] No se puede suscribir: boss es null");
            return;
        }

        Enemy bossEnemy = boss.GetComponent<Enemy>();
        if (bossEnemy == null)
        {
            Debug.LogError($"[LevelManager] ❌ Boss '{boss.name}' no tiene componente Enemy.cs");
            return;
        }

        // Suscribir al evento OnDeath
        bossEnemy.OnDeath.AddListener(OnBossDefeated);
        Debug.Log("[LevelManager] Suscrito al evento OnDeath del boss");
    }

    /// <summary>
    /// Llamado cuando el boss muere
    /// </summary>
    void OnBossDefeated()
    {
        if (bossDefeated)
        {
            Debug.LogWarning("[LevelManager] OnBossDefeated ya fue llamado, ignorando...");
            return;
        }

        bossDefeated = true;
        Debug.Log($"[LevelManager] 🎉 ¡BOSS DERROTADO! Nivel {levelNumber} completado");

        // 1. Añadir puntos por boss + bonus de nivel
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.AddScore(pointsPerBoss + levelCompletionBonus);
        }

        // 2. Actualizar progreso si es nuevo récord
        UpdateProgress();

        // 3. Mostrar panel de victoria
        ShowVictoryPanel();
    }

    /// <summary>
    /// Actualiza el progreso en ProgressManager y sincroniza con backend
    /// </summary>
    void UpdateProgress()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogError("[LevelManager] ❌ ProgressManager no encontrado");
            return;
        }

        int currentScore = ProgressManager.Instance.currentScore;

        // ProgressManager.UpdateProgress() verifica internamente si es nuevo récord
        ProgressManager.Instance.UpdateProgress(levelNumber, currentScore);
    }

    /// <summary>
    /// Mostrar panel de victoria
    /// </summary>
    void ShowVictoryPanel()
    {
        if (victoryPanel == null)
        {
            Debug.LogWarning("[LevelManager] ⚠️ VictoryPanel no asignado en Inspector");
            Debug.LogWarning("[LevelManager] Cargando selector de niveles automáticamente...");
            Invoke("ReturnToMenu", 2f); // Volver al menú después de 2 segundos
            return;
        }

        VictoryPanel panel = victoryPanel.GetComponent<VictoryPanel>();
        if (panel != null)
        {
            // Mostrar victoria con recompensas
            // TODO: Obtener coins e item del EnemyDropSystem
            int coinsEarned = 100; // Placeholder
            string itemName = ""; // Placeholder
            int nextLevel = levelNumber + 1;

            panel.ShowVictory(coinsEarned, itemName, nextLevel);
        }
        else
        {
            Debug.LogError("[LevelManager] ❌ VictoryPanel no tiene componente VictoryPanel.cs");
            victoryPanel.SetActive(true); // Al menos mostrarlo
        }
    }

    /// <summary>
    /// Cargar siguiente nivel
    /// Llamado por VictoryPanel cuando se clickea "Siguiente Nivel"
    /// </summary>
    public void LoadNextLevel()
{
    int nextLevel = levelNumber + 1;

    if (nextLevel <= 5)
    {
        Debug.Log($"[LevelManager] Cargando Nivel {nextLevel}...");
        SceneManager.LoadScene($"Level_{nextLevel}");
    }
    else
    {
        Debug.Log("[LevelManager] 🎊 ¡JUEGO COMPLETADO! Todos los niveles superados");
        ReturnToMenu();
    }
}

/// <summary>
/// Volver al selector de niveles
/// Llamado por VictoryPanel cuando se clickea "Menú"
/// </summary>
public void ReturnToMenu()
{
    Debug.Log("[LevelManager] Volviendo al selector de niveles...");
    SceneManager.LoadScene("LevelSelector");
}

/// <summary>
/// Llamado cuando el jugador mata un enemigo normal (no boss)
/// Puedes llamar esto desde el script de enemigos normales si quieres
/// </summary>
public void OnEnemyKilled()
{
    if (ProgressManager.Instance != null)
    {
        ProgressManager.Instance.AddScore(pointsPerEnemy);
    }
}
}
