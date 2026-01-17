using UnityEngine;
using System.Collections;

/// <summary>
/// Singleton que mantiene el progreso del jugador durante toda la sesión
/// - ActFrag (nivel máximo alcanzado 0-5)
/// - BestScore (mejor puntuación)
/// - CurrentBytes (monedas actuales)
/// - Username
/// Este objeto NO se destruye al cambiar de escena (DontDestroyOnLoad)
/// </summary>
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    [Header("Información del Jugador")]
    public string username;

    [Header("Progreso del Juego")]
    [Tooltip("Nivel máximo alcanzado (ActFrag en backend). 0 = ninguno completado, 5 = todos completados")]
    public int maxLevelReached = 0;

    [Tooltip("Mejor puntuación de todos los tiempos")]
    public int bestScore = 0;

    [Tooltip("Monedas/Bytes actuales del jugador")]
    public int currentBytes = 0;

    [Header("Score de Partida Actual")]
    [Tooltip("Puntuación de la partida actual (se resetea al cambiar de nivel)")]
    public int currentScore = 0;

    [Header("Configuración")]
    [Tooltip("Total de niveles del juego")]
    public int totalLevels = 5;

    void Awake()
    {
        // Singleton pattern con DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[ProgressManager] Inicializado (DontDestroyOnLoad)");
        }
        else
        {
            Debug.LogWarning("[ProgressManager] Ya existe una instancia, destruyendo duplicado");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Cargar datos desde el perfil del backend
    /// Llamado por GameLauncher después de obtener el JSON
    /// </summary>
    public void LoadFromProfile(UnityProfileResponse profile)
    {
        if (profile == null || profile.user == null)
        {
            Debug.LogError("[ProgressManager] Perfil nulo, no se puede cargar progreso");
            return;
        }

        username = profile.user.username;
        maxLevelReached = profile.user.actFrag;
        bestScore = profile.user.bestScore;
        currentBytes = profile.user.monedas;

        Debug.Log($"[ProgressManager] Progreso cargado:");
        Debug.Log($"  - Username: {username}");
        Debug.Log($"  - Nivel Máximo: {maxLevelReached}/{totalLevels}");
        Debug.Log($"  - Mejor Score: {bestScore}");
        Debug.Log($"  - Bytes: {currentBytes}");
    }

    /// <summary>
    /// Verifica si un nivel está desbloqueado
    /// Nivel 1 siempre desbloqueado
    /// Nivel N desbloqueado si ActFrag >= N-1
    /// </summary>
    public bool IsLevelUnlocked(int levelNumber)
    {
        // Nivel 1 siempre disponible
        if (levelNumber == 1) return true;

        // Nivel N desbloqueado si has completado el nivel N-1
        // Ejemplo: ActFrag=2 → niveles 1,2,3 desbloqueados
        return levelNumber <= maxLevelReached + 1;
    }

    /// <summary>
    /// Actualiza el progreso después de completar un nivel
    /// Solo actualiza si es nuevo récord
    /// Sincroniza automáticamente con el backend
    /// </summary>
    public void UpdateProgress(int newLevelCompleted, int newScore)
    {
        bool updated = false;

        // Actualizar ActFrag si es nuevo récord
        if (newLevelCompleted > maxLevelReached)
        {
            Debug.Log($"[ProgressManager] ¡Nuevo nivel completado! {maxLevelReached} → {newLevelCompleted}");
            maxLevelReached = newLevelCompleted;
            updated = true;
        }

        // Actualizar BestScore si superaste tu récord
        if (newScore > bestScore)
        {
            Debug.Log($"[ProgressManager] ¡Nuevo récord de puntuación! {bestScore} → {newScore}");
            bestScore = newScore;
            updated = true;
        }

        // Sincronizar con backend si hubo cambios
        if (updated)
        {
            if (APIManager.Instance != null)
            {
                StartCoroutine(APIManager.Instance.UpdateProgress(maxLevelReached, bestScore));
                Debug.Log("[ProgressManager] ✅ Progreso sincronizado con backend");
            }
            else
            {
                Debug.LogError("[ProgressManager] ❌ APIManager no encontrado, no se puede sincronizar");
            }
        }
        else
        {
            Debug.Log("[ProgressManager] Sin cambios en progreso (rejugando nivel completado)");
        }
    }

    /// <summary>
    /// Añadir puntos al score actual (partida en curso)
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        Debug.Log($"[ProgressManager] Score actual: {currentScore} (+{points})");
    }

    /// <summary>
    /// Resetear score de partida actual (llamar al iniciar nuevo nivel)
    /// </summary>
    public void ResetCurrentScore()
    {
        currentScore = 0;
        Debug.Log("[ProgressManager] Score de partida reseteado");
    }

    /// <summary>
    /// Añadir bytes/monedas (llamar cuando se obtienen drops)
    /// </summary>
    public void AddBytes(int amount)
    {
        currentBytes += amount;
        Debug.Log($"[ProgressManager] Bytes actuales: {currentBytes} (+{amount})");
    }
}
