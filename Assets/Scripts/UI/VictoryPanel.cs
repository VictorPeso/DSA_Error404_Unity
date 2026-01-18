using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Panel de victoria que se muestra al derrotar al boss
/// Muestra recompensas (monedas, items) y botones para continuar
/// Guarda progreso en el backend antes de cambiar de escena
/// </summary>
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

    void Start()
    {
        // Ocultar panel al inicio (se muestra solo cuando se derrota al boss)
        gameObject.SetActive(false);

        // Configurar botones
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    /// <summary>
    /// Muestra el panel de victoria con información de recompensas
    /// </summary>
    public void ShowVictory(int coinsEarned, string itemName, int nextLevel)
    {
        this.coinsEarned = coinsEarned;
        this.lootObjectId = itemName; // En realidad es el objectId (ej: "obj01")
        this.levelCompleted = nextLevel - 1; // Si nextLevel=2, completó el nivel 1
        nextLevelNumber = nextLevel;

        // Actualizar textos
        if (titleText != null)
        {
            titleText.text = "¡SECTOR LIBERADO!";
        }

        if (coinsText != null)
        {
            coinsText.text = $"Bytes obtenidos: {coinsEarned}";
        }

        if (itemText != null)
        {
            if (!string.IsNullOrEmpty(itemName))
            {
                itemText.text = $"Item obtenido: {itemName}";
            }
            else
            {
                itemText.text = "Sin item";
            }
        }

        // Configurar botón de siguiente nivel
        if (nextLevelButton != null)
        {
            // Si ya completó todos los niveles, deshabilitar botón
            if (nextLevelNumber > 5) // Asumiendo 5 niveles totales
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

        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
        
        Debug.Log($"[VictoryPanel] Nivel {levelCompleted} completado. Coins: {coinsEarned}, Loot: {lootObjectId}");
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
        
        // NOTA: Las monedas y el loot YA fueron guardados por EnemyDropSystem al morir el boss
        // Solo necesitamos guardar las pociones usadas y el progreso del nivel
        
        // 1. Guardar pociones usadas
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
                    
                    // Encontrar el item actual en el inventario para saber la cantidad restante
                    EquippedItem item = EquipmentManager.Instance.inventory
                        .Find(i => i.objectId == objectId);
                    
                    if (item != null)
                    {
                        int newQuantity = item.cantidad; // Ya está actualizado localmente
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
        
        // 2. Actualizar actFrag y bestScore
        if (ProgressManager.Instance != null)
        {
            int currentScore = ProgressManager.Instance.currentScore;
            Debug.Log($"[VictoryPanel] Guardando progreso: actFrag={levelCompleted}, bestScore={currentScore}");
            
            // Esto ya llama internamente a APIManager.UpdateProgress()
            ProgressManager.Instance.UpdateProgress(levelCompleted, currentScore);
            
            // Esperar un frame para que se complete la llamada
            yield return null;
        }
        
        Debug.Log("[VictoryPanel] ========== PROGRESO GUARDADO EXITOSAMENTE ==========");
    }
}

