using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && inventoryPanel != null && inventoryPanel.activeSelf)
        {
            CloseInventory();
        }
    }

    public void OpenInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogWarning("[InventoryUI] inventoryPanel no asignado");
            return;
        }

        if (hudCanvas != null)
        {
            hudCanvas.SetActive(false);
        }

        Time.timeScale = 0f;
        inventoryPanel.SetActive(true);
        RefreshInventory();
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (hudCanvas != null)
        {
            hudCanvas.SetActive(true);
        }

        Time.timeScale = 1f;
    }

    void RefreshInventory()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("[InventoryUI] EquipmentManager no encontrado");
            return;
        }

        UpdateStatsDisplay();
        RebuildWeaponsList();
        RebuildArmorsList();
        RebuildConsumablesList();
    }

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

    void RebuildWeaponsList()
    {
        if (weaponsListParent == null) return;

        ClearChildren(weaponsListParent);
        List<EquippedItem> weapons = EquipmentManager.Instance.GetWeapons();

        if (weapons.Count == 0)
        {
            CreateEmptyMessage(weaponsListParent, "No tienes armas");
            return;
        }

        foreach (EquippedItem weapon in weapons)
        {
            CreateWeaponButton(weapon, weaponsListParent);
        }
    }

    void RebuildArmorsList()
    {
        if (armorsListParent == null) return;

        ClearChildren(armorsListParent);
        List<EquippedItem> armors = EquipmentManager.Instance.GetArmors();

        if (armors.Count == 0)
        {
            CreateEmptyMessage(armorsListParent, "No tienes armaduras");
            return;
        }

        foreach (EquippedItem armor in armors)
        {
            CreateArmorButton(armor, armorsListParent);
        }
    }

    void RebuildConsumablesList()
    {
        if (consumablesListParent == null) return;

        ClearChildren(consumablesListParent);
        List<EquippedItem> potions = EquipmentManager.Instance.GetPotions();

        Debug.Log($"[InventoryUI.RebuildConsumablesList] Pociones a mostrar: {potions.Count}");

        if (potions.Count == 0)
        {
            CreateEmptyMessage(consumablesListParent, "No tienes consumibles");
            return;
        }

        foreach (EquippedItem potion in potions)
        {
            Debug.Log($"[InventoryUI.RebuildConsumablesList] Creando botón para: {potion.nombre} (cantidad: {potion.cantidad})");
            CreatePotionButton(potion, consumablesListParent);
        }
    }

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

    GameObject CreateItemButton(string itemName, string itemInfo, string actionText, bool actionEnabled, System.Action onClickAction, Transform parent)
    {
        GameObject itemObj = new GameObject($"Item_{itemName}");
        itemObj.transform.SetParent(parent, false);
        
        RectTransform itemRect = itemObj.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, 80);
        
        LayoutElement itemLayout = itemObj.AddComponent<LayoutElement>();
        itemLayout.minHeight = 80;
        itemLayout.preferredHeight = 80;
        itemLayout.flexibleWidth = 1;
        
        HorizontalLayoutGroup layout = itemObj.AddComponent<HorizontalLayoutGroup>();
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.spacing = 10;
        layout.padding = new RectOffset(10, 10, 5, 5);

        GameObject infoPanel = new GameObject("InfoPanel");
        infoPanel.transform.SetParent(itemObj.transform, false);
        RectTransform infoPanelRect = infoPanel.AddComponent<RectTransform>();
        LayoutElement infoLayout = infoPanel.AddComponent<LayoutElement>();
        infoLayout.flexibleWidth = 1;

        VerticalLayoutGroup infoVLayout = infoPanel.AddComponent<VerticalLayoutGroup>();
        infoVLayout.childForceExpandWidth = true;
        infoVLayout.childForceExpandHeight = false;
        infoVLayout.childControlWidth = true;
        infoVLayout.childControlHeight = true;

        GameObject nameTextObj = new GameObject("NameText");
        nameTextObj.transform.SetParent(infoPanel.transform, false);
        TextMeshProUGUI nameText = nameTextObj.AddComponent<TextMeshProUGUI>();
        nameText.text = $"• {itemName}";
        nameText.fontSize = 18;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.Left;
        nameText.enableWordWrapping = false;
        nameText.overflowMode = TextOverflowModes.Overflow;

        GameObject statsTextObj = new GameObject("StatsText");
        statsTextObj.transform.SetParent(infoPanel.transform, false);
        TextMeshProUGUI statsTextComp = statsTextObj.AddComponent<TextMeshProUGUI>();
        statsTextComp.text = itemInfo;
        statsTextComp.fontSize = 14;
        statsTextComp.color = new Color(0.7f, 0.7f, 0.7f);
        statsTextComp.alignment = TextAlignmentOptions.Left;
        statsTextComp.enableWordWrapping = false;
        statsTextComp.overflowMode = TextOverflowModes.Overflow;

        GameObject buttonObj = new GameObject("ActionButton");
        buttonObj.transform.SetParent(itemObj.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        LayoutElement buttonLayoutElem = buttonObj.AddComponent<LayoutElement>();
        buttonLayoutElem.minWidth = 100;
        buttonLayoutElem.preferredWidth = 100;
        
        Button button = buttonObj.AddComponent<Button>();
        button.interactable = actionEnabled;
        button.onClick.AddListener(() => onClickAction?.Invoke());

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = actionEnabled ? new Color(0.2f, 0.6f, 0.2f) : new Color(0.3f, 0.3f, 0.3f);

        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = actionText;
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.enableWordWrapping = false;
        buttonText.overflowMode = TextOverflowModes.Overflow;
        
        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;

        return itemObj;
    }

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

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    string GetItemStatsText(EquippedItem item)
    {
        switch (item.objectId)
        {
            case "obj05": return "+10 DMG";
            case "obj09": return "+25 DMG";
            case "obj14": return "+40 DMG +10% CRIT";
            case "obj01": return "+50 DMG";
            case "obj06": return "+8 DEF";
            case "obj10": return "+20 DEF +30 HP";
            case "obj11": return "+12 DEF";
            case "obj02": return "+25 DEF";
            case "obj15": return "+35 DEF +50 HP";
            case "obj18": return "+60 DEF +100 HP";
            case "obj04": return "+25 HP";
            default: return "???";
        }
    }

    void OnEquipWeapon(EquippedItem weapon)
    {
        if (weapon == null) return;

        EquipmentManager.Instance.EquipItem(weapon);
        RefreshInventory();
    }

    void OnEquipArmor(EquippedItem armor)
    {
        if (armor == null) return;

        EquipmentManager.Instance.EquipItem(armor);
        RefreshInventory();
    }

    void OnUsePotion(EquippedItem potion)
    {
        if (potion == null) return;

        bool success = EquipmentManager.Instance.UsePotion();
        
        if (success)
        {
            RefreshInventory();
        }
    }
}
