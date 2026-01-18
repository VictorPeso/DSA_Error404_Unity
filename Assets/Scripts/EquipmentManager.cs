using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EquippedItem
{
    public string objectId;
    public string nombre;
    public string tipo;
    public int cantidad;
}

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("Slots de Equipamiento")]
    public EquippedItem espada;
    public EquippedItem armadura;

    [Header("Inventario del Usuario")]
    public List<EquippedItem> inventory = new List<EquippedItem>();

    [Header("Pociones Usadas (Para Sincronizar con Backend)")]
    private Dictionary<string, int> potionsUsedThisLevel = new Dictionary<string, int>();

    [Header("Stats de la IA (se calculan automáticamente)")]
    public int totalDamage = 10;
    public int totalDefense = 5;
    public int totalHP = 100;
    public float critChance = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadInventoryFromBackend(GameObjectData[] objects)
    {
        inventory.Clear();
        
        // ADD DEBUG LOG
        Debug.Log($"[EquipmentManager] Recibidos {objects?.Length ?? 0} objetos del backend");
        
        if (objects == null || objects.Length == 0)
        {
            Debug.LogWarning("[EquipmentManager] No hay objetos para cargar en el inventario");
            return;
        }
        
        foreach (GameObjectData obj in objects)
        {
            // ADD DEBUG LOG
            Debug.Log($"[EquipmentManager] Objeto: id={obj.id}, nombre={obj.nombre}, tipo={obj.tipo}, cantidad={obj.cantidad}");
            
            EquippedItem item = new EquippedItem
            {
                objectId = obj.id,
                nombre = obj.nombre,
                tipo = obj.tipo,
                cantidad = obj.cantidad
            };
            
            inventory.Add(item);
        }
        
        // ADD DEBUG LOG
        Debug.Log($"[EquipmentManager] Total en inventario: {inventory.Count}");
        RecalculateStats();
    }

    public void LoadInventoryFromJSON(string profileJSON)
    {
        Debug.LogWarning("[EquipmentManager] LoadInventoryFromJSON es legacy, usa LoadInventoryFromBackend");
        inventory.Clear();
        RecalculateStats();
    }

    public void EquipItem(EquippedItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[EquipmentManager] Intento de equipar item null");
            return;
        }

        switch (item.tipo)
        {
            case "ARMA":
                espada = item;
                break;
            
            case "ARMADURA":
                armadura = item;
                break;
            
            default:
                Debug.LogWarning($"[EquipmentManager] Tipo no equipable: {item.tipo}");
                return;
        }

        RecalculateStats();
    }

    public void UnequipSlot(string slotType)
    {
        switch (slotType)
        {
            case "ARMA": espada = null; break;
            case "ARMADURA": armadura = null; break;
        }

        RecalculateStats();
    }

    void RecalculateStats()
    {
        totalDamage = 10;
        totalDefense = 5;
        totalHP = 100;
        critChance = 0f;

        ApplyItemStats(espada);
        ApplyItemStats(armadura);
    }

    void ApplyItemStats(EquippedItem item)
    {
        if (item == null) return;

        switch (item.objectId)
        {
            case "obj04": break;
            case "obj05": totalDamage += 10; break;
            case "obj06": totalDefense += 8; break;
            case "obj09": totalDamage += 25; break;
            case "obj10": totalDefense += 20; totalHP += 30; break;
            case "obj11": totalDefense += 12; break;
            case "obj02": totalDefense += 25; break;
            case "obj14": totalDamage += 40; critChance += 10f; break;
            case "obj15": totalDefense += 35; totalHP += 50; break;
            case "obj01": totalDamage += 50; break;
            case "obj18": totalDefense += 60; totalHP += 100; break;
            default:
                Debug.LogWarning($"[EquipmentManager] Objeto sin stats definidos: {item.objectId}");
                break;
        }
    }

    public bool UsePotion()
    {
        EquippedItem potion = GetFirstPotion();
        
        if (potion == null)
        {
            return false;
        }

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.RestoreHealth(25);
            potion.cantidad--;
            
            // Trackear la poción usada para sincronizar con el backend
            if (!potionsUsedThisLevel.ContainsKey(potion.objectId))
            {
                potionsUsedThisLevel[potion.objectId] = 0;
            }
            potionsUsedThisLevel[potion.objectId]++;
            
            Debug.Log($"[EquipmentManager] Poción {potion.objectId} usada. Total usadas este nivel: {potionsUsedThisLevel[potion.objectId]}");
            
            return true;
        }
        else
        {
            Debug.LogWarning("[EquipmentManager] No se encontró PlayerHealth en la escena");
            return false;
        }
    }

    public int GetPotionCount()
    {
        int count = 0;
        foreach (EquippedItem item in inventory)
        {
            if (item.tipo == "POCION" && item.cantidad > 0)
            {
                count += item.cantidad;
            }
        }
        return count;
    }

    public EquippedItem GetFirstPotion()
    {
        foreach (EquippedItem item in inventory)
        {
            if (item.tipo == "POCION" && item.cantidad > 0)
            {
                return item;
            }
        }
        return null;
    }

    public List<EquippedItem> GetWeapons()
    {
        List<EquippedItem> weapons = new List<EquippedItem>();
        foreach (EquippedItem item in inventory)
        {
            if (item.tipo == "ARMA")
            {
                weapons.Add(item);
            }
        }
        return weapons;
    }

    public List<EquippedItem> GetArmors()
    {
        List<EquippedItem> armors = new List<EquippedItem>();
        foreach (EquippedItem item in inventory)
        {
            if (item.tipo == "ARMADURA")
            {
                armors.Add(item);
            }
        }
        return armors;
    }

    public List<EquippedItem> GetPotions()
    {
        List<EquippedItem> potions = new List<EquippedItem>();
        Debug.Log($"[EquipmentManager.GetPotions] Buscando pociones en inventario. Total items: {inventory.Count}");
        
        foreach (EquippedItem item in inventory)
        {
            Debug.Log($"[EquipmentManager.GetPotions] Revisando item: id={item.objectId}, tipo={item.tipo}, cantidad={item.cantidad}");
            
            if (item.tipo == "POCION")
            {
                Debug.Log($"[EquipmentManager.GetPotions] ✓ Poción encontrada: {item.nombre}");
                potions.Add(item);
            }
        }
        
        Debug.Log($"[EquipmentManager.GetPotions] Total pociones encontradas: {potions.Count}");
        return potions;
    }

    public bool IsEquipped(EquippedItem item)
    {
        if (item == null) return false;
        
        if (item.tipo == "ARMA" && espada != null && espada.objectId == item.objectId)
        {
            return true;
        }
        
        if (item.tipo == "ARMADURA" && armadura != null && armadura.objectId == item.objectId)
        {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Obtiene las pociones usadas durante este nivel y reinicia el contador.
    /// Retorna un diccionario con objectId -> cantidad usada
    /// </summary>
    public Dictionary<string, int> GetAndResetPotionsUsed()
    {
        var used = new Dictionary<string, int>(potionsUsedThisLevel);
        potionsUsedThisLevel.Clear();
        Debug.Log($"[EquipmentManager] Reseteando contador de pociones. Total tipos usados: {used.Count}");
        return used;
    }

    /// <summary>
    /// Reinicia el contador de pociones usadas (llamar al inicio de cada nivel)
    /// </summary>
    public void ResetPotionsUsedCounter()
    {
        potionsUsedThisLevel.Clear();
        Debug.Log("[EquipmentManager] Contador de pociones reseteado");
    }
}

