using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Botón de HUD para usar pociones rápidamente
/// Se actualiza automáticamente mostrando la cantidad disponible
/// Compatible con controles táctiles para Android
/// </summary>
public class QuickPotionButton : MonoBehaviour
{
    [Header("Referencias UI")]
    public Button potionButton;
    public TextMeshProUGUI countText;

    void Start()
    {
        // Verificar que las referencias estén asignadas
        if (potionButton == null)
        {
            potionButton = GetComponentInChildren<Button>();
            if (potionButton == null)
            {
                Debug.LogError("[QuickPotionButton] No se encontró el componente Button");
            }
        }

        if (countText == null)
        {
            Debug.LogWarning("[QuickPotionButton] countText no asignado. No se mostrará la cantidad.");
        }
    }

    void Update()
    {
        // Verificar si EquipmentManager está disponible
        if (EquipmentManager.Instance == null) return;

        // Obtener cantidad de pociones disponibles
        int count = EquipmentManager.Instance.GetPotionCount();

        // Actualizar texto de cantidad
        if (countText != null)
        {
            countText.text = $"x{count}";
        }

        // Habilitar/deshabilitar botón según disponibilidad
        if (potionButton != null)
        {
            potionButton.interactable = (count > 0);
        }
    }

    /// <summary>
    /// Callback del botón - Usa una poción
    /// Este método se llama desde el evento OnClick del botón en el Inspector
    /// </summary>
    public void OnPotionButtonClick()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogWarning("[QuickPotionButton] EquipmentManager no disponible");
            return;
        }

        bool success = EquipmentManager.Instance.UsePotion();
        
        if (success)
        {
            Debug.Log("[QuickPotionButton] ✅ Poción usada (+25 HP)");
        }
        else
        {
            Debug.Log("[QuickPotionButton] ❌ No tienes pociones disponibles");
        }
    }
}
