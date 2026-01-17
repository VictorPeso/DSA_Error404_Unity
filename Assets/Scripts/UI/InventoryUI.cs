using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Sistema de inventario para Android
/// Muestra equipamiento actual, lista de items por categoría
/// Permite equipar armas/armaduras y usar pociones
/// SOLO controles táctiles (sin teclado)
/// VERSIÓN SIMPLIFICADA: Crea botones dinámicamente SIN necesitar prefab
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Panel Principal")]
    public GameObject inventoryPanel;
    public Button closeButton;
    public GameObject hudCanvas;  // Referencia al Canvas PlayerUI (HUD)

    [Header("Listas de Items")]
    public Transform weaponsListParent;
    public Transform armorsListParent;
    public Transform consumablesListParent;

    [Header("Stats")]
    public TextMeshProUGUI statsText;

    void Start()
    {
        // Ocultar panel al inicio
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        // Conectar botón de cerrar
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
    }

    void Update()
    {
        // Detectar botón "Atrás" de Android (o ESC en PC) para cerrar inventario
        if (Input.GetKeyDown(KeyCode.Escape) && inventoryPanel != null && inventoryPanel.activeSelf)
        {
            CloseInventory();
        }
    }

    /// <summary>
    /// Abrir el panel de inventario
    /// Llamado por el botón "Abrir Inventario" del HUD
    /// </summary>
    public void OpenInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogWarning("[InventoryUI] inventoryPanel no asignado");
            return;
        }

        // Ocultar HUD
        if (hudCanvas != null)
        {
            hudCanvas.SetActive(false);
            Debug.Log("[InventoryUI] 🔒 HUD ocultado");
        }

        // Pausar el juego (congelar enemigos)
        Time.timeScale = 0f;
        Debug.Log("[InventoryUI] ⏸️ Juego pausado");

        // Mostrar inventario
        inventoryPanel.SetActive(true);
        RefreshInventory();
        Debug.Log("[InventoryUI] 📦 Inventario abierto");
    }

    /// <summary>
    /// Cerrar el panel de inventario
    /// </summary>
    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("[InventoryUI] ❌ Inventario cerrado");
        }

        // Mostrar HUD de nuevo
        if (hudCanvas != null)
        {
            hudCanvas.SetActive(true);
            Debug.Log("[InventoryUI] 🔓 HUD visible");
        }

        // Reanudar el juego
        Time.timeScale = 1f;
        Debug.Log("[InventoryUI] ▶️ Juego reanudado");
    }

    /// <summary>
    /// Refrescar toda la UI del inventario
    /// Reconstruye las listas de items dinámicamente
    /// </summary>
    void RefreshInventory()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("[InventoryUI] EquipmentManager no encontrado");
            return;
        }

        // 1. Actualizar stats
        UpdateStatsDisplay();

        // 2. Reconstruir listas de items
        RebuildWeaponsList();
        RebuildArmorsList();
        RebuildConsumablesList();
    }

    /// <summary>
    /// Actualizar display de stats
    /// </summary>
    void UpdateStatsDisplay()
    {
        if (statsText != null)
        {
            statsText.text = $"DMG: {EquipmentManager.Instance.totalDamage} | " +
                           $"DEF: {EquipmentManager.Instance.totalDefense} | " +
                           $"HP: {EquipmentManager.Instance.totalHP}";
            
            if (EquipmentManager.Instance.critChance > 0)
            {
                statsText.text += $" | CRIT: {EquipmentManager.Instance.critChance}%";
            }
        }
    }

    /// <summary>
    /// Reconstruir lista de armas
    /// </summary>
    void RebuildWeaponsList()
    {
        if (weaponsListParent == null) return;

        // Limpiar botones existentes
        ClearChildren(weaponsListParent);

        // Obtener armas del inventario
        List<EquippedItem> weapons = EquipmentManager.Instance.GetWeapons();

        if (weapons.Count == 0)
        {
            CreateEmptyMessage(weaponsListParent, "No tienes armas");
            return;
        }

        // Crear botón para cada arma
        foreach (EquippedItem weapon in weapons)
        {
            CreateWeaponButton(weapon, weaponsListParent);
        }
    }

    /// <summary>
    /// Reconstruir lista de armaduras
    /// </summary>
    void RebuildArmorsList()
    {
        if (armorsListParent == null) return;

        // Limpiar botones existentes
        ClearChildren(armorsListParent);

        // Obtener armaduras del inventario
        List<EquippedItem> armors = EquipmentManager.Instance.GetArmors();

        if (armors.Count == 0)
        {
            CreateEmptyMessage(armorsListParent, "No tienes armaduras");
            return;
        }

        // Crear botón para cada armadura
        foreach (EquippedItem armor in armors)
        {
            CreateArmorButton(armor, armorsListParent);
        }
    }

    /// <summary>
    /// Reconstruir lista de consumibles
    /// </summary>
    void RebuildConsumablesList()
    {
        if (consumablesListParent == null) return;

        // Limpiar botones existentes
        ClearChildren(consumablesListParent);

        // Obtener pociones del inventario
        List<EquippedItem> potions = EquipmentManager.Instance.GetPotions();

        if (potions.Count == 0)
        {
            CreateEmptyMessage(consumablesListParent, "No tienes consumibles");
            return;
        }

        // Crear botón para cada poción
        foreach (EquippedItem potion in potions)
        {
            CreatePotionButton(potion, consumablesListParent);
        }
    }

    /// <summary>
    /// Crear botón de arma dinámicamente
    /// </summary>
    void CreateWeaponButton(EquippedItem weapon, Transform parent)
    {
        bool isEquipped = EquipmentManager.Instance.IsEquipped(weapon);
        
        GameObject buttonObj = CreateItemButton(
            weapon.nombre,
            GetItemStatsText(weapon),
            isEquipped ? "✓ EQUIPADO" : "EQUIPAR",
            !isEquipped,
            () => OnEquipWeapon(weapon),
            parent
        );
    }

    /// <summary>
    /// Crear botón de armadura dinámicamente
    /// </summary>
    void CreateArmorButton(EquippedItem armor, Transform parent)
    {
        bool isEquipped = EquipmentManager.Instance.IsEquipped(armor);
        
        GameObject buttonObj = CreateItemButton(
            armor.nombre,
            GetItemStatsText(armor),
            isEquipped ? "✓ EQUIPADO" : "EQUIPAR",
            !isEquipped,
            () => OnEquipArmor(armor),
            parent
        );
    }

    /// <summary>
    /// Crear botón de poción dinámicamente
    /// </summary>
    void CreatePotionButton(EquippedItem potion, Transform parent)
    {
        GameObject buttonObj = CreateItemButton(
            potion.nombre,
            $"x{potion.cantidad}",
            "USAR",
            potion.cantidad > 0,
            () => OnUsePotion(potion),
            parent
        );
    }

    /// <summary>
    /// Crear un botón de item genérico (programáticamente, sin prefab)
    /// </summary>
    GameObject CreateItemButton(string itemName, string itemInfo, string actionText, bool actionEnabled, System.Action onClickAction, Transform parent)
    {
        // Crear GameObject contenedor del item
        GameObject itemObj = new GameObject($"Item_{itemName}");
        itemObj.transform.SetParent(parent, false);
        
        RectTransform itemRect = itemObj.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, 80); // Alto de 80 pixels
        
        // Añadir LayoutElement para asegurar tamaño mínimo
        LayoutElement itemLayout = itemObj.AddComponent<LayoutElement>();
        itemLayout.minHeight = 80;
        itemLayout.preferredHeight = 80;
        itemLayout.flexibleWidth = 1; // Usa todo el ancho disponible
        
        // Layout horizontal para organizar contenido
        HorizontalLayoutGroup layout = itemObj.AddComponent<HorizontalLayoutGroup>();
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.spacing = 10;
        layout.padding = new RectOffset(10, 10, 5, 5);

        // === PANEL IZQUIERDO: Info del item ===
        GameObject infoPanel = new GameObject("InfoPanel");
        infoPanel.transform.SetParent(itemObj.transform, false);
        RectTransform infoPanelRect = infoPanel.AddComponent<RectTransform>();
        LayoutElement infoLayout = infoPanel.AddComponent<LayoutElement>();
        infoLayout.flexibleWidth = 1; // Toma espacio disponible

        VerticalLayoutGroup infoVLayout = infoPanel.AddComponent<VerticalLayoutGroup>();
        infoVLayout.childForceExpandWidth = true;
        infoVLayout.childForceExpandHeight = false;
        infoVLayout.childControlWidth = true;
        infoVLayout.childControlHeight = true;

        // Nombre del item
        GameObject nameTextObj = new GameObject("NameText");
        nameTextObj.transform.SetParent(infoPanel.transform, false);
        TextMeshProUGUI nameText = nameTextObj.AddComponent<TextMeshProUGUI>();
        nameText.text = $"• {itemName}";
        nameText.fontSize = 18;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.Left;
        nameText.enableWordWrapping = false; // No hacer wrap
        nameText.overflowMode = TextOverflowModes.Overflow; // Permitir overflow horizontal

        // Stats/Info del item
        GameObject statsTextObj = new GameObject("StatsText");
        statsTextObj.transform.SetParent(infoPanel.transform, false);
        TextMeshProUGUI statsTextComp = statsTextObj.AddComponent<TextMeshProUGUI>();
        statsTextComp.text = itemInfo;
        statsTextComp.fontSize = 14;
        statsTextComp.color = new Color(0.7f, 0.7f, 0.7f);
        statsTextComp.alignment = TextAlignmentOptions.Left;
        statsTextComp.enableWordWrapping = false; // No hacer wrap
        statsTextComp.overflowMode = TextOverflowModes.Overflow; // Permitir overflow horizontal

        // === BOTÓN DERECHO: Acción (EQUIPAR/USAR) ===
        GameObject buttonObj = new GameObject("ActionButton");
        buttonObj.transform.SetParent(itemObj.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        LayoutElement buttonLayoutElem = buttonObj.AddComponent<LayoutElement>();
        buttonLayoutElem.minWidth = 100;
        buttonLayoutElem.preferredWidth = 100;
        
        Button button = buttonObj.AddComponent<Button>();
        button.interactable = actionEnabled;
        button.onClick.AddListener(() => onClickAction?.Invoke());

        // Fondo del botón
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = actionEnabled ? new Color(0.2f, 0.6f, 0.2f) : new Color(0.3f, 0.3f, 0.3f);

        // Texto del botón
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = actionText;
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.enableWordWrapping = false; // No hacer wrap
        buttonText.overflowMode = TextOverflowModes.Overflow; // Permitir overflow
        
        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;

        return itemObj;
    }

    /// <summary>
    /// Crear mensaje de lista vacía
    /// </summary>
    void CreateEmptyMessage(Transform parent, string message)
    {
        GameObject textObj = new GameObject("EmptyMessage");
        textObj.transform.SetParent(parent, false);
        
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 40);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 16;
        text.color = new Color(0.5f, 0.5f, 0.5f);
        text.alignment = TextAlignmentOptions.Center;
    }

    /// <summary>
    /// Limpiar todos los hijos de un transform
    /// </summary>
    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Obtener texto de stats de un item
    /// </summary>
    string GetItemStatsText(EquippedItem item)
    {
        switch (item.objectId)
        {
            // ARMAS
            case "obj05": return "+10 DMG";
            case "obj09": return "+25 DMG";
            case "obj14": return "+40 DMG +10% CRIT";
            case "obj01": return "+50 DMG";
            
            // ARMADURAS
            case "obj06": return "+8 DEF";
            case "obj10": return "+20 DEF +30 HP";
            case "obj11": return "+12 DEF";
            case "obj02": return "+25 DEF";
            case "obj15": return "+35 DEF +50 HP";
            case "obj18": return "+60 DEF +100 HP";
            
            // POCIONES
            case "obj04": return "+25 HP";
            
            default: return "???";
        }
    }

    // ========== CALLBACKS ==========

    /// <summary>
    /// Callback cuando se equipa un arma
    /// </summary>
    void OnEquipWeapon(EquippedItem weapon)
    {
        if (weapon == null) return;

        EquipmentManager.Instance.EquipItem(weapon);
        RefreshInventory();
        
        Debug.Log($"[InventoryUI] ⚔️ Arma equipada: {weapon.nombre}");
    }

    /// <summary>
    /// Callback cuando se equipa una armadura
    /// </summary>
    void OnEquipArmor(EquippedItem armor)
    {
        if (armor == null) return;

        EquipmentManager.Instance.EquipItem(armor);
        RefreshInventory();
        
        Debug.Log($"[InventoryUI] 🛡️ Armadura equipada: {armor.nombre}");
    }

    /// <summary>
    /// Callback cuando se usa una poción
    /// </summary>
    void OnUsePotion(EquippedItem potion)
    {
        if (potion == null) return;

        bool success = EquipmentManager.Instance.UsePotion();
        
        if (success)
        {
            RefreshInventory();
            Debug.Log($"[InventoryUI] 💊 Poción usada: {potion.nombre} (+25 HP)");
        }
        else
        {
            Debug.Log($"[InventoryUI] ❌ No se pudo usar la poción");
        }
    }
}
