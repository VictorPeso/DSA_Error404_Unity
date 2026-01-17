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

/// <summary>
/// Gestor del sistema de equipamiento de la IA
/// Maneja slots de equipo y recalcula stats automáticamente
/// TEMÁTICA: IA luchando contra virus en un servidor infectado
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("Slots de Equipamiento")]
    public EquippedItem espada;
    public EquippedItem armadura;

    [Header("Inventario del Usuario")]
    public List<EquippedItem> inventory = new List<EquippedItem>();

    [Header("Stats de la IA (se calculan automáticamente)")]
    public int totalDamage = 10;      // Base: 10 (Daño contra virus)
    public int totalDefense = 5;      // Base: 5 (Defensa contra ataques)
    public int totalHP = 100;         // Base: 100 (Vida de la IA)
    public float critChance = 0f;     // Base: 0% (Crítico)

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

    /// <summary>
    /// Cargar inventario desde el backend (objetos del usuario)
    /// Llamado por GameLauncher después de obtener el perfil
    /// </summary>
    public void LoadInventoryFromBackend(GameObjectData[] objects)
    {
        inventory.Clear();
        
        if (objects == null || objects.Length == 0)
        {
            Debug.Log("[EquipmentManager] Usuario no tiene objetos en el inventario");
            return;
        }
        
        // Convertir GameObjectData[] a EquippedItem[]
        foreach (GameObjectData obj in objects)
        {
            EquippedItem item = new EquippedItem
            {
                objectId = obj.id,
                nombre = obj.nombre,
                tipo = obj.tipo,
                cantidad = obj.cantidad
            };
            
            inventory.Add(item);
            Debug.Log($"[EquipmentManager] ✅ Objeto cargado: {obj.nombre} (x{obj.cantidad}) - {obj.tipo}");
        }
        
        Debug.Log($"[EquipmentManager] 📦 Total objetos cargados: {inventory.Count}");
        
        // Recalcular stats por si el jugador tenía objetos equipados
        RecalculateStats();
    }

    /// <summary>
    /// Llamado por GameLauncher cuando se descarga el perfil
    /// MÉTODO LEGACY - Mantener por compatibilidad pero usar LoadInventoryFromBackend
    /// </summary>
    public void LoadInventoryFromJSON(string profileJSON)
    {
        // Este método ya no se usa, pero lo mantenemos por compatibilidad
        // GameLauncher ahora usa LoadInventoryFromBackend directamente
        Debug.LogWarning("[EquipmentManager] LoadInventoryFromJSON es legacy, usa LoadInventoryFromBackend");
        
        inventory.Clear();
        Debug.Log("[EquipmentManager] Inventario inicializado (método legacy)");
        RecalculateStats();
    }

    /// <summary>
    /// Equipar un objeto en el slot correspondiente (solo ESPADA y ARMADURA)
    /// </summary>
    public void EquipItem(EquippedItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[EquipmentManager] Intento de equipar item null");
            return;
        }

        switch (item.tipo)
        {
            case "ESPADA":
                espada = item;
                Debug.Log($"[EquipmentManager] ⚔️ Espada equipada: {item.nombre}");
                break;
            
            case "ARMADURA":
                armadura = item;
                Debug.Log($"[EquipmentManager] 🛡️ Armadura equipada: {item.nombre}");
                break;
            
            default:
                Debug.LogWarning($"[EquipmentManager] Tipo no equipable: {item.tipo}");
                return;
        }

        RecalculateStats();
    }

    /// <summary>
    /// Desequipar un slot (solo ESPADA y ARMADURA)
    /// </summary>
    public void UnequipSlot(string slotType)
    {
        switch (slotType)
        {
            case "ESPADA": espada = null; break;
            case "ARMADURA": armadura = null; break;
        }

        Debug.Log($"[EquipmentManager] Slot {slotType} desequipado");
        RecalculateStats();
    }

    /// <summary>
    /// Recalcular stats según equipamiento actual (solo ESPADA y ARMADURA)
    /// </summary>
    void RecalculateStats()
    {
        // Stats base de la IA
        totalDamage = 10;
        totalDefense = 5;
        totalHP = 100;
        critChance = 0f;

        // Aplicar bonuses de cada objeto equipado
        ApplyItemStats(espada);
        ApplyItemStats(armadura);

        Debug.Log($"[EquipmentManager] 📊 Stats actualizados → DMG:{totalDamage} DEF:{totalDefense} HP:{totalHP} CRIT:{critChance}%");
    }

    void ApplyItemStats(EquippedItem item)
    {
        if (item == null) return;

        // Tabla de stats según ID del objeto (TEMÁTICA: IA vs VIRUS)
        switch (item.objectId)
        {
            // COMÚN (Herramientas básicas)
            case "obj04": /* Patch de Emergencia - no afecta stats pasivos */ break;
            case "obj05": totalDamage += 10; break;  // Scanner Básico
            case "obj06": totalDefense += 8; break;  // Firewall Portátil

            // RARO (Software profesional)
            case "obj09": totalDamage += 25; break;  // Antivirus Pro
            case "obj10": totalDefense += 20; totalHP += 30; break;  // Encriptación AES
            case "obj11": totalDefense += 12; break;  // Casco de Red Segura

            // ÉPICO (Tecnología avanzada)
            case "obj02": totalDefense += 25; break;  // Firewall Corporativo
            case "obj14": totalDamage += 40; critChance += 10f; break;  // Scanner Cuántico
            case "obj15": totalDefense += 35; totalHP += 50; break;  // Firewall Neural

            // LEGENDARIO (Tech militar)
            case "obj01": totalDamage += 50; break;  // Antivirus Militar
            case "obj18": totalDefense += 60; totalHP += 100; break;  // Sistema TITAN

            default:
                Debug.LogWarning($"[EquipmentManager] Objeto sin stats definidos: {item.objectId}");
                break;
        }
    }

    /// <summary>
    /// Usar una poción de emergencia (busca en inventario, decrementa cantidad)
    /// </summary>
    public bool UsePotion()
    {
        // Buscar la primera poción con cantidad > 0
        EquippedItem potion = GetFirstPotion();
        
        if (potion == null)
        {
            Debug.Log("[EquipmentManager] ❌ No tienes pociones disponibles");
            return false;
        }

        // Buscar el PlayerHealth en la escena
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.RestoreHealth(25);
            potion.cantidad--;
            
            Debug.Log($"[EquipmentManager] 💊 Poción usada: {potion.nombre} (+25 HP) - Quedan: {potion.cantidad}");
            return true;
        }
        else
        {
            Debug.LogWarning("[EquipmentManager] ❌ No se encontró PlayerHealth en la escena");
            return false;
        }
    }

    /// <summary>
    /// Obtener el número total de pociones disponibles
    /// </summary>
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

    /// <summary>
    /// Obtener la primera poción con cantidad > 0
    /// </summary>
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

    /// <summary>
    /// Obtener lista de todas las armas en el inventario
    /// </summary>
    public List<EquippedItem> GetWeapons()
    {
        List<EquippedItem> weapons = new List<EquippedItem>();
        foreach (EquippedItem item in inventory)
        {
            if (item.tipo == "ESPADA")
            {
                weapons.Add(item);
            }
        }
        return weapons;
    }

    /// <summary>
    /// Obtener lista de todas las armaduras en el inventario
    /// </summary>
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

    /// <summary>
    /// Obtener lista de todas las pociones en el inventario
    /// </summary>
    public List<EquippedItem> GetPotions()
    {
        List<EquippedItem> potions = new List<EquippedItem>();
        foreach (EquippedItem item in inventory)
        {
            if (item.tipo == "POCION")
            {
                potions.Add(item);
            }
        }
        return potions;
    }

    /// <summary>
    /// Verificar si un item está equipado actualmente
    /// </summary>
    public bool IsEquipped(EquippedItem item)
    {
        if (item == null) return false;
        
        if (item.tipo == "ESPADA" && espada != null && espada.objectId == item.objectId)
        {
            return true;
        }
        
        if (item.tipo == "ARMADURA" && armadura != null && armadura.objectId == item.objectId)
        {
            return true;
        }
        
        return false;
    }
}
