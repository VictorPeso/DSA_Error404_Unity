using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class VictoryPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button menuButton;

    [Header("Configuration")]
    [SerializeField] private string levelSelectorSceneName = "LevelSelector";

    private int nextLevelNumber;
    private int coinsEarned;
    private string lootObjectId;
    private int levelCompleted;
    private bool isSaving = false;
    private bool shouldShowPanel = false;

    void Awake()
    {
        Debug.Log("[VictoryPanel] Awake() llamado");
    }

    void Start()
    {
        if (gameObject.activeSelf && !shouldShowPanel)
        {
            Debug.LogWarning("[VictoryPanel] Panel estaba activo al inicio sin razón, desactivando...");
            gameObject.SetActive(false);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuClicked);
        }

        Debug.Log($"[VictoryPanel] Start() llamado. Panel activo: {gameObject.activeSelf}");
    }

    public void ShowVictory(int coinsEarned, string itemName, int nextLevel)
    {
        Debug.Log($"[VictoryPanel] ShowVictory() llamado. Panel activo antes: {gameObject.activeSelf}");

        shouldShowPanel = true;

        if (gameObject.activeSelf)
        {
            Debug.LogWarning("[VictoryPanel] El panel ya está activo, ignorando llamada duplicada");
            return;
        }

        this.coinsEarned = coinsEarned;
        this.lootObjectId = itemName;
        this.levelCompleted = nextLevel - 1;
        nextLevelNumber = nextLevel;

        Debug.Log("[VictoryPanel] Actualizando textos...");

        if (coinsText != null)
        {
            coinsText.text = $"Bytes: {coinsEarned}";
        }
        else
        {
            Debug.LogWarning("[VictoryPanel] coinsText es NULL");
        }

        if (itemText != null)
        {
            if (!string.IsNullOrEmpty(itemName))
            {
                itemText.text = $"Item: {itemName}";
            }
            else
            {
                itemText.text = "Sin item";
            }
        }
        else
        {
            Debug.LogWarning("[VictoryPanel] itemText es NULL");
        }

        if (nextLevelButton != null)
        {
            if (nextLevelNumber > 5)
            {
                nextLevelButton.interactable = false;
                if (nextLevelButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                {
                    nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "¡Juego Completado!";
                }
            }
            else
            {
                nextLevelButton.interactable = true;
            }
        }
        else
        {
            Debug.LogWarning("[VictoryPanel] nextLevelButton es NULL");
        }

        Debug.Log("[VictoryPanel] Activando panel...");
        gameObject.SetActive(true);
        Debug.Log($"[VictoryPanel] Panel activo después de SetActive: {gameObject.activeSelf}");

        Debug.Log("[VictoryPanel] Pausando juego (Time.timeScale = 0)");
        Time.timeScale = 0f;

        Debug.Log($"[VictoryPanel] ✅ Nivel {levelCompleted} completado. Coins: {coinsEarned}, Loot: {lootObjectId}");
    }

    void OnNextLevelClicked()
    {
        if (isSaving)
        {
            Debug.LogWarning("[VictoryPanel] Ya se está guardando, ignorando click");
            return;
        }

        StartCoroutine(SaveProgressAndLoadNextLevel());
    }

    void OnMenuClicked()
    {
        if (isSaving)
        {
            Debug.LogWarning("[VictoryPanel] Ya se está guardando, ignorando click");
            return;
        }

        StartCoroutine(SaveProgressAndReturnToMenu());
    }

    IEnumerator SaveProgressAndLoadNextLevel()
    {
        isSaving = true;
        yield return StartCoroutine(SaveAllProgress());

        Time.timeScale = 1f;
        string nextSceneName = $"Level_{nextLevelNumber}";

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning($"[VictoryPanel] Escena '{nextSceneName}' no encontrada, volviendo al selector de niveles");
            SceneManager.LoadScene(levelSelectorSceneName);
        }
    }

    IEnumerator SaveProgressAndReturnToMenu()
    {
        isSaving = true;
        yield return StartCoroutine(SaveAllProgress());

        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelectorSceneName);
    }

    IEnumerator SaveAllProgress()
    {
        Debug.Log("[VictoryPanel] ========== INICIANDO GUARDADO DE PROGRESO ==========");

        if (EquipmentManager.Instance != null && APIManager.Instance != null)
        {
            var potionsUsed = EquipmentManager.Instance.GetAndResetPotionsUsed();

            if (potionsUsed.Count > 0)
            {
                Debug.Log($"[VictoryPanel] Guardando {potionsUsed.Count} tipos de pociones usadas...");

                foreach (var kvp in potionsUsed)
                {
                    string objectId = kvp.Key;
                    int usedCount = kvp.Value;

                    EquippedItem item = EquipmentManager.Instance.inventory
                        .Find(i => i.objectId == objectId);

                    if (item != null)
                    {
                        int newQuantity = item.cantidad;
                        Debug.Log($"[VictoryPanel] Actualizando {objectId}: cantidad={newQuantity} (usadas: {usedCount})");

                        yield return StartCoroutine(
                            APIManager.Instance.UpdateObjectQuantity(objectId, newQuantity)
                        );
                    }
                    else
                    {
                        Debug.LogWarning($"[VictoryPanel] Item {objectId} no encontrado en inventario");
                    }
                }
            }
            else
            {
                Debug.Log("[VictoryPanel] No se usaron pociones en este nivel");
            }
        }

        if (ProgressManager.Instance != null)
        {
            int currentScore = ProgressManager.Instance.currentScore;
            Debug.Log($"[VictoryPanel] Guardando progreso: actFrag={levelCompleted}, bestScore={currentScore}");

            // actualziar progreso
            ProgressManager.Instance.UpdateProgress(levelCompleted, currentScore);

            yield return null;
        }

        Debug.Log("[VictoryPanel] ========== PROGRESO GUARDADO EXITOSAMENTE ==========");
    }
}

