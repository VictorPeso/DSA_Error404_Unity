using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickPotionButton : MonoBehaviour
{
    [Header("Referencias UI")]
    public Button potionButton;
    public TextMeshProUGUI countText;

    void Start()
    {
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
        if (EquipmentManager.Instance == null) return;

        int count = EquipmentManager.Instance.GetPotionCount();

        if (countText != null)
        {
            countText.text = $"x{count}";
        }

        if (potionButton != null)
        {
            potionButton.interactable = (count > 0);
        }
    }

    public void OnPotionButtonClick()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogWarning("[QuickPotionButton] EquipmentManager no disponible");
            return;
        }

        EquipmentManager.Instance.UsePotion();
    }
}
