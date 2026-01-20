using UnityEngine;
using System.Collections;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    [Header("Información del Jugador")]
    public string username;

    [Header("Progreso del Juego")]
    [Tooltip("Nivel máximo alcanzado")]
    public int maxLevelReached = 0;

    [Tooltip("Mejor puntuación de todos los tiempos")]
    public int bestScore = 0;

    [Tooltip("Monedas/Bytes actuales del jugador")]
    public int currentBytes = 0;

    [Header("Score de Partida Actual")]
    [Tooltip("Puntuación de la partida actual")]
    public int currentScore = 0;

    [Header("Configuración")]
    [Tooltip("Total de niveles del juego")]
    public int totalLevels = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("[ProgressManager] Ya existe una instancia, destruyendo duplicado");
            Destroy(gameObject);
        }
    }

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
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber == 1) return true;
        return levelNumber <= maxLevelReached + 1;
    }

    public void UpdateProgress(int newLevelCompleted, int newScore)
    {
        bool updated = false;

        if (newLevelCompleted > maxLevelReached)
        {
            maxLevelReached = newLevelCompleted;
            updated = true;
        }

        if (newScore > bestScore)
        {
            bestScore = newScore;
            updated = true;
        }

        if (updated)
        {
            if (APIManager.Instance != null)
            {
                StartCoroutine(APIManager.Instance.UpdateProgress(maxLevelReached, bestScore));
            }
            else
            {
                Debug.LogError("[ProgressManager] APIManager no encontrado, no se puede sincronizar");
            }
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
    }

    public void ResetCurrentScore()
    {
        currentScore = 0;
    }

    public void AddBytes(int amount)
    {
        currentBytes += amount;
    }
}
