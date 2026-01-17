using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// UI del selector de niveles
/// SOLO bloquea/desbloquea botones según progreso
/// Los botones cargan escenas mediante OnClick configurado manualmente en Inspector
/// </summary>
public class LevelSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bytesText;
    [SerializeField] private Button[] levelButtons; // Array de 5 botones (Level 1-5)

    [Header("Button Sprites (Optional)")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    void Start()
    {
        StartCoroutine(InitializeAfterDelay());
    }

    /// <summary>
    /// Espera un frame para asegurar que ProgressManager está disponible
    /// </summary>
    System.Collections.IEnumerator InitializeAfterDelay()
    {
        // Esperar hasta que ProgressManager esté disponible
        int attempts = 0;
        while (ProgressManager.Instance == null && attempts < 10)
        {
            yield return null; // Espera 1 frame
            attempts++;
        }

        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[LevelSelectorUI] ProgressManager no encontrado - todos los botones desbloqueados");
            UnlockAllButtons();
            yield break;
        }

        // ProgressManager está disponible, configurar bloqueo/desbloqueo
        ConfigureLevelButtons();
        UpdateInfoTexts();
    }

    /// <summary>
    /// Desbloquea todos los botones si no hay ProgressManager
    /// </summary>
    void UnlockAllButtons()
    {
        if (levelButtons == null) return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = true;
                Debug.Log($"[LevelSelectorUI] Nivel {i + 1}: Desbloqueado (sin ProgressManager)");
            }
        }

        // Actualizar textos con valores por defecto
        if (progressText != null) progressText.text = "Progreso: 0/5";
        if (scoreText != null) scoreText.text = "Mejor Score: 0";
        if (bytesText != null) bytesText.text = "Bytes: 0";
    }

    /// <summary>
    /// Actualiza los textos de información con datos de ProgressManager
    /// </summary>
    void UpdateInfoTexts()
    {
        if (ProgressManager.Instance == null) return;

        int maxLevel = ProgressManager.Instance.maxLevelReached;
        int bestScore = ProgressManager.Instance.bestScore;
        int bytes = ProgressManager.Instance.currentBytes;

        if (progressText != null)
        {
            progressText.text = $"Progreso: {maxLevel}/5";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Mejor Score: {bestScore:N0}";
        }

        if (bytesText != null)
        {
            bytesText.text = $"Bytes: {bytes}";
        }

        Debug.Log($"[LevelSelectorUI] Stats actualizados - MaxLevel: {maxLevel}, Score: {bestScore}, Bytes: {bytes}");
    }

    /// <summary>
    /// SOLO configura si los botones están bloqueados o desbloqueados
    /// NO configura OnClick - se hace manualmente en Inspector
    /// </summary>
    void ConfigureLevelButtons()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("[LevelSelectorUI] No hay botones de nivel asignados");
            return;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null) continue;

            int levelNumber = i + 1; // Nivel 1-5
            bool isUnlocked = ProgressManager.Instance.IsLevelUnlocked(levelNumber);

            // SOLO configurar si está bloqueado o no
            levelButtons[i].interactable = isUnlocked;

            // Cambiar sprite si está disponible
            Image buttonImage = levelButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                if (isUnlocked && unlockedSprite != null)
                {
                    buttonImage.sprite = unlockedSprite;
                }
                else if (!isUnlocked && lockedSprite != null)
                {
                    buttonImage.sprite = lockedSprite;
                }
            }

            Debug.Log($"[LevelSelectorUI] Nivel {levelNumber}: {(isUnlocked ? "✅ Desbloqueado" : "🔒 Bloqueado")}");
        }
    }
}
