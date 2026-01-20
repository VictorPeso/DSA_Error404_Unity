using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bytesText;
    [SerializeField] private Button[] levelButtons;

    [Header("Button Sprites (Optional)")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    void Start()
    {
        StartCoroutine(InitializeAfterDelay());
    }

    System.Collections.IEnumerator InitializeAfterDelay()
    {
        int attempts = 0;
        while (ProgressManager.Instance == null && attempts < 10)
        {
            yield return null;
            attempts++;
        }

        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[LevelSelectorUI] ProgressManager no encontrado - todos los botones desbloqueados");
            UnlockAllButtons();
            yield break;
        }

        ConfigureLevelButtons();
        UpdateInfoTexts();
    }

    void UnlockAllButtons()
    {
        if (levelButtons == null) return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = true;
            }
        }

        if (progressText != null) progressText.text = "Progreso: 0/5";
        if (scoreText != null) scoreText.text = "Mejor Score: 0";
        if (bytesText != null) bytesText.text = "Bytes: 0";
    }

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
    }

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

            int levelNumber = i + 1;
            bool isUnlocked = ProgressManager.Instance.IsLevelUnlocked(levelNumber);

            levelButtons[i].interactable = isUnlocked;

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
        }
    }
}
