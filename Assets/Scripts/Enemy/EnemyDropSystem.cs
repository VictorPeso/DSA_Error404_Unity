using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de drops para enemigos
/// NOTA: Este script se añade como componente adicional al Enemy.cs existente
/// Se activa automáticamente cuando el enemigo muere
/// </summary>
[RequireComponent(typeof(Enemy))]
public class EnemyDropSystem : MonoBehaviour
{
    [Header("Tipo de Enemigo")]
    [Tooltip("Marcar si este enemigo es un jefe (drop de objetos)")]
    public bool isBoss = false;

    [Header("Recompensas - Monedas")]
    public int minCoins = 10;
    public int maxCoins = 30;
    public int bossMinCoins = 100;
    public int bossMaxCoins = 200;

    [Header("Recompensas - Boss Drops")]
    [Range(0f, 1f)]
    [Tooltip("Probabilidad de que el jefe suelte un objeto (0.9 = 90%)")]
    public float dropProbability = 0.9f; // 90% de probabilidad

    // Tabla de drops por rareza (configurar según tu BD)
    private LootTable lootTable = new LootTable();
    private Enemy enemyScript;

    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        SetupLootTable();
    }

    void Update()
    {
        // Detectar si el enemigo ha muerto
        // Verificamos si el GameObject está a punto de ser destruido o ya no tiene vida
        if (enemyScript == null && gameObject != null)
        {
            OnEnemyDeath();
        }
    }

    void OnDestroy()
    {
        // Cuando el enemigo se destruye, procesamos drops
        OnEnemyDeath();
    }

    void SetupLootTable()
    {
        // COMÚN (50% probabilidad)
        lootTable.AddItem("obj04", 0.50f, Rarity.Common);  // Poción Pequeña
        lootTable.AddItem("obj05", 0.50f, Rarity.Common);  // Daga Básica
        lootTable.AddItem("obj06", 0.50f, Rarity.Common);  // Escudo de Madera

        // RARO (30% probabilidad)
        lootTable.AddItem("obj09", 0.30f, Rarity.Rare);    // Espada de Acero
        lootTable.AddItem("obj10", 0.30f, Rarity.Rare);    // Armadura de Hierro
        lootTable.AddItem("obj11", 0.30f, Rarity.Rare);    // Casco de Soldado

        // ÉPICO (15% probabilidad)
        lootTable.AddItem("obj02", 0.15f, Rarity.Epic);    // Escudo Mágico (original)
        lootTable.AddItem("obj14", 0.15f, Rarity.Epic);    // Espada Encantada
        lootTable.AddItem("obj15", 0.15f, Rarity.Epic);    // Escudo de Dragón

        // LEGENDARIO (5% probabilidad)
        lootTable.AddItem("obj01", 0.05f, Rarity.Legendary); // Espada de Fuego
        lootTable.AddItem("obj18", 0.05f, Rarity.Legendary); // Armadura del Titán
    }

    void OnEnemyDeath()
    {
        // Evitar ejecutar múltiples veces
        if (hasProcessedDeath) return;
        hasProcessedDeath = true;

        // 1. MONEDAS (todos los enemigos)
        int coins = isBoss ? 
            Random.Range(bossMinCoins, bossMaxCoins) : 
            Random.Range(minCoins, maxCoins);
        
        string enemyType = isBoss ? "🔥 JEFE" : "Enemigo";
        Debug.Log($"[EnemyDrops] {enemyType} eliminado. Monedas: {coins}");
        
        if (APIManager.Instance != null)
        {
            StartCoroutine(APIManager.Instance.AddCoins(coins));
        }
        else
        {
            Debug.LogWarning("[EnemyDrops] APIManager no encontrado. Monedas no guardadas.");
        }

        // 2. DROP DE OBJETO (solo jefes con probabilidad)
        if (isBoss)
        {
            float roll = Random.value; // 0.0 a 1.0
            
            if (roll <= dropProbability) // 90% por defecto
            {
                LootItem drop = lootTable.RollLoot();
                
                if (drop != null)
                {
                    string rarityColor = GetRarityColorTag(drop.rarity);
                    Debug.Log($"[EnemyDrops] 🎁 ¡DROP! {rarityColor}{drop.itemId} ({drop.rarity})</color>");
                    
                    if (APIManager.Instance != null)
                    {
                        StartCoroutine(APIManager.Instance.AddBossLoot(drop.itemId));
                    }

                    // Efecto visual según rareza (opcional)
                    ShowDropEffect(drop);
                }
            }
            else
            {
                Debug.Log("[EnemyDrops] 😢 El jefe no soltó nada esta vez... (mala suerte)");
            }
        }
    }

    private bool hasProcessedDeath = false;

    void ShowDropEffect(LootItem drop)
    {
        // Aquí puedes instanciar partículas según la rareza
        Color effectColor = GetRarityParticleColor(drop.rarity);
        Debug.Log($"[EnemyDrops] Mostrando efecto de drop con color: {effectColor}");
        
        // Ejemplo: Instantiate(dropEffectPrefab, transform.position, Quaternion.identity);
        // Y cambiar el color del ParticleSystem según effectColor
    }

    string GetRarityColorTag(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return "<color=grey>";
            case Rarity.Rare: return "<color=blue>";
            case Rarity.Epic: return "<color=purple>";
            case Rarity.Legendary: return "<color=orange>";
            default: return "";
        }
    }

    Color GetRarityParticleColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return Color.grey;
            case Rarity.Rare: return Color.blue;
            case Rarity.Epic: return new Color(0.5f, 0f, 0.5f); // Púrpura
            case Rarity.Legendary: return new Color(1f, 0.5f, 0f); // Naranja
            default: return Color.white;
        }
    }
}

// ============= CLASES DE SOPORTE =============

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class LootItem
{
    public string itemId;
    public float dropChance;
    public Rarity rarity;

    public LootItem(string id, float chance, Rarity rarity)
    {
        this.itemId = id;
        this.dropChance = chance;
        this.rarity = rarity;
    }
}

public class LootTable
{
    private List<LootItem> items = new List<LootItem>();

    public void AddItem(string itemId, float dropChance, Rarity rarity)
    {
        items.Add(new LootItem(itemId, dropChance, rarity));
    }

    public LootItem RollLoot()
    {
        if (items.Count == 0) return null;

        // Sistema de dos pasos: primero elegir rareza, luego objeto
        Rarity rolledRarity = RollRarity();
        
        // Filtrar objetos de esa rareza
        List<LootItem> possibleItems = items.FindAll(item => item.rarity == rolledRarity);
        
        if (possibleItems.Count == 0) return null;

        // Elegir uno aleatorio de esa rareza
        return possibleItems[Random.Range(0, possibleItems.Count)];
    }

    Rarity RollRarity()
    {
        float roll = Random.value; // 0.0 a 1.0

        // Distribución acumulativa:
        // 0.00 - 0.50 = Común (50%)
        // 0.50 - 0.80 = Raro (30%)
        // 0.80 - 0.95 = Épico (15%)
        // 0.95 - 1.00 = Legendario (5%)

        if (roll < 0.50f) return Rarity.Common;
        if (roll < 0.80f) return Rarity.Rare;
        if (roll < 0.95f) return Rarity.Epic;
        return Rarity.Legendary;
    }
}
