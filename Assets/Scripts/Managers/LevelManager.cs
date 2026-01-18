using UnityEngine;
using UnityEngine.SceneManagement;

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
        FindReferencesIfNeeded();

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetCurrentScore();
        }

        TeleportPlayerToSpawn();
        SubscribeToBossDeathEvent();
    }

    void FindReferencesIfNeeded()
    {
        if (spawnRoom == null)
        {
            spawnRoom = GameObject.FindWithTag("Respawn");
            if (spawnRoom == null)
            {
                Debug.LogWarning("[LevelManager] No se encontró GameObject con tag 'Respawn'");
            }
        }

        if (boss == null)
        {
            boss = GameObject.FindWithTag("Boss");
            if (boss == null)
            {
                Debug.LogWarning("[LevelManager] No se encontró GameObject con tag 'Boss'");
            }
        }
    }

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
            Debug.LogError("[LevelManager] No se encontró GameObject con tag 'Player'");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = spawnRoom.transform.position;
        player.transform.rotation = spawnRoom.transform.rotation;

        if (cc != null) cc.enabled = true;
    }

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
            Debug.LogError($"[LevelManager] Boss '{boss.name}' no tiene componente Enemy.cs");
            return;
        }

        bossEnemy.OnDeath.AddListener(OnBossDefeated);
    }

    void OnBossDefeated()
    {
        if (bossDefeated)
        {
            Debug.LogWarning("[LevelManager] OnBossDefeated ya fue llamado, ignorando...");
            return;
        }

        bossDefeated = true;

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.AddScore(pointsPerBoss + levelCompletionBonus);
        }

        UpdateProgress();
        ShowVictoryPanel();
    }

    void UpdateProgress()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogError("[LevelManager] ProgressManager no encontrado");
            return;
        }

        int currentScore = ProgressManager.Instance.currentScore;
        ProgressManager.Instance.UpdateProgress(levelNumber, currentScore);
    }

    void ShowVictoryPanel()
    {
        if (victoryPanel == null)
        {
            Debug.LogWarning("[LevelManager] VictoryPanel no asignado en Inspector");
            Invoke("ReturnToMenu", 2f);
            return;
        }

        VictoryPanel panel = victoryPanel.GetComponent<VictoryPanel>();
        if (panel != null)
        {
            // TODO: Obtener coins e item del EnemyDropSystem
            int coinsEarned = 100;
            string itemName = "";
            int nextLevel = levelNumber + 1;

            panel.ShowVictory(coinsEarned, itemName, nextLevel);
        }
        else
        {
            Debug.LogError("[LevelManager] VictoryPanel no tiene componente VictoryPanel.cs");
            victoryPanel.SetActive(true);
        }
    }

    public void LoadNextLevel()
{
    int nextLevel = levelNumber + 1;

    if (nextLevel <= 5)
    {
        SceneManager.LoadScene($"Level_{nextLevel}");
    }
    else
    {
        ReturnToMenu();
    }
}

public void ReturnToMenu()
{
    SceneManager.LoadScene("LevelSelector");
}

public void OnEnemyKilled()
{
    if (ProgressManager.Instance != null)
    {
        ProgressManager.Instance.AddScore(pointsPerEnemy);
    }
}
}
