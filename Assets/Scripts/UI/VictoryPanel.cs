using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Panel de victoria que se muestra al derrotar al boss
/// Muestra recompensas (monedas, items) y botones para continuar
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
    }

    void OnNextLevelClicked()
    {
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

    void OnMenuClicked()
    {
        SceneManager.LoadScene(levelSelectorSceneName);
    }
}
