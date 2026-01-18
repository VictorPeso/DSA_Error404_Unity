using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    [Tooltip("Número del nivel actual")]
    public int levelNumber = 1;

    [Tooltip("Nombre descriptivo del nivel")]
    public string levelName = "Sector 1";

    [Header("Referencias de Salas")]
    [Tooltip("Punto de spawn del jugador")]
    public GameObject spawnRoom;

    [Tooltip("GameObject del Boss")]
    public GameObject boss;

    [Header("UI")]
    [Tooltip("Panel de victoria")]
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

        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.ResetPotionsUsedCounter();
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

        if (victoryPanel == null)
        {
            Debug.Log("[LevelManager] VictoryPanel no asignado, buscando en la escena...");

            VictoryPanel panelScript = FindObjectOfType<VictoryPanel>();
            if (panelScript != null)
            {
                victoryPanel = panelScript.gameObject;
                Debug.Log($"[LevelManager] ✅ VictoryPanel encontrado automáticamente: {victoryPanel.name}");
            }
            else
            {
                Debug.LogError("[LevelManager] ❌ No se encontró ningún GameObject con componente VictoryPanel en la escena!");
                Debug.LogError("[LevelManager] SOLUCIÓN: Asegúrate de que hay un panel UI con el script VictoryPanel.cs en la escena");
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
        Debug.Log("[LevelManager] Intentando suscribirse al evento de muerte del boss...");

        if (boss == null)
        {
            Debug.LogWarning("[LevelManager] ❌ Boss es NULL, no se puede suscribir");
            Debug.LogWarning("[LevelManager] SOLUCIÓN: Arrastra el GameObject del boss al campo 'Boss' en Inspector");
            return;
        }

        Debug.Log($"[LevelManager] Boss encontrado: {boss.name}");

        Enemy bossEnemy = boss.GetComponent<Enemy>();
        if (bossEnemy == null)
        {
            Debug.LogError($"[LevelManager] ❌ Boss '{boss.name}' NO tiene componente Enemy.cs");
            Debug.LogError("[LevelManager] SOLUCIÓN: Añade el script Enemy.cs al GameObject del boss");
            return;
        }

        Debug.Log("[LevelManager] Componente Enemy encontrado, añadiendo listener...");
        bossEnemy.OnDeath.AddListener(OnBossDefeated);
        Debug.Log("[LevelManager] ✅ Suscripción exitosa al evento OnDeath del boss");
    }

    void OnBossDefeated()
    {
        Debug.Log("========== [LevelManager] BOSS DERROTADO ==========");

        if (bossDefeated)
        {
            Debug.LogWarning("[LevelManager] OnBossDefeated ya fue llamado, ignorando...");
            return;
        }

        bossDefeated = true;

        if (boss != null)
        {
            Enemy bossEnemy = boss.GetComponent<Enemy>();
            if (bossEnemy != null)
            {
                bossEnemy.OnDeath.RemoveListener(OnBossDefeated);
                Debug.Log("[LevelManager] Desuscrito del evento OnDeath del boss");
            }
        }

        Debug.Log($"[LevelManager] Añadiendo puntos: {pointsPerBoss + levelCompletionBonus}");

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.AddScore(pointsPerBoss + levelCompletionBonus);
        }

        UpdateProgress();
        Debug.Log("[LevelManager] Mostrando panel de victoria...");
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
        Debug.Log("[LevelManager] ========== ShowVictoryPanel() llamado ==========");

        if (victoryPanel == null)
        {
            Debug.LogError("[LevelManager] ❌ VictoryPanel NO está asignado en Inspector!");
            Debug.LogError("[LevelManager] SOLUCIÓN: Arrastra el GameObject del panel al campo 'Victory Panel'");
            Invoke("ReturnToMenu", 2f);
            return;
        }

        Debug.Log($"[LevelManager] VictoryPanel encontrado: {victoryPanel.name}");
        Debug.Log($"[LevelManager] VictoryPanel activo antes: {victoryPanel.activeSelf}");
        Debug.Log($"[LevelManager] VictoryPanel transform parent: {(victoryPanel.transform.parent != null ? victoryPanel.transform.parent.name : "null")}");

        VictoryPanel panel = victoryPanel.GetComponent<VictoryPanel>();
        if (panel != null)
        {

            int coinsDisplayed = 100;
            string itemDisplayed = "";
            int nextLevel = levelNumber + 1;

            Debug.Log($"[LevelManager] Llamando panel.ShowVictory() con nextLevel={nextLevel}");
            panel.ShowVictory(coinsDisplayed, itemDisplayed, nextLevel);
            Debug.Log($"[LevelManager] VictoryPanel activo después de ShowVictory: {victoryPanel.activeSelf}");
        }
        else
        {
            Debug.LogError($"[LevelManager] ❌ VictoryPanel '{victoryPanel.name}' NO tiene componente VictoryPanel.cs!");
            Debug.LogError("[LevelManager] SOLUCIÓN: Añade el script VictoryPanel.cs al GameObject");
            Debug.Log("[LevelManager] Intentando activar panel manualmente sin script...");
            victoryPanel.SetActive(true);
            Debug.Log($"[LevelManager] VictoryPanel activo después de SetActive manual: {victoryPanel.activeSelf}");
        }

        Debug.Log("[LevelManager] ========== ShowVictoryPanel() completado ==========");
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
