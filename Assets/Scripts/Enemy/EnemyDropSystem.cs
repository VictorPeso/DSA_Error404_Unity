using UnityEngine;
using System.Collections.Generic;

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
    public float dropProbability = 0.9f;

    private LootTable lootTable = new LootTable();
    private Enemy enemyScript;
    private bool hasProcessedDeath = false;

    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        SetupLootTable();
    }

    void Update()
    {
        if (enemyScript == null && gameObject != null)
        {
            OnEnemyDeath();
        }
    }

    void OnDestroy()
    {
        OnEnemyDeath();
    }

    void SetupLootTable()
    {
        lootTable.AddItem("obj04", 0.50f, Rarity.Common);
        lootTable.AddItem("obj05", 0.50f, Rarity.Common);
        lootTable.AddItem("obj06", 0.50f, Rarity.Common);

        lootTable.AddItem("obj09", 0.30f, Rarity.Rare);
        lootTable.AddItem("obj10", 0.30f, Rarity.Rare);
        lootTable.AddItem("obj11", 0.30f, Rarity.Rare);

        lootTable.AddItem("obj02", 0.15f, Rarity.Epic);
        lootTable.AddItem("obj14", 0.15f, Rarity.Epic);
        lootTable.AddItem("obj15", 0.15f, Rarity.Epic);

        lootTable.AddItem("obj01", 0.05f, Rarity.Legendary);
        lootTable.AddItem("obj18", 0.05f, Rarity.Legendary);
    }

    void OnEnemyDeath()
    {
        if (hasProcessedDeath) return;
        hasProcessedDeath = true;

        int coins = isBoss ? 
            Random.Range(bossMinCoins, bossMaxCoins) : 
            Random.Range(minCoins, maxCoins);
        
        if (APIManager.Instance != null)
        {
            StartCoroutine(APIManager.Instance.AddCoins(coins));
        }
        else
        {
            Debug.LogWarning("[EnemyDrops] APIManager no encontrado. Monedas no guardadas.");
        }

        if (isBoss)
        {
            float roll = Random.value;
            
            if (roll <= dropProbability)
            {
                LootItem drop = lootTable.RollLoot();
                
                if (drop != null)
                {
                    if (APIManager.Instance != null)
                    {
                        StartCoroutine(APIManager.Instance.AddBossLoot(drop.itemId));
                    }

                    ShowDropEffect(drop);
                }
            }
        }
    }

    void ShowDropEffect(LootItem drop)
    {
        Color effectColor = GetRarityParticleColor(drop.rarity);
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
            case Rarity.Epic: return new Color(0.5f, 0f, 0.5f);
            case Rarity.Legendary: return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }
}

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

        Rarity rolledRarity = RollRarity();
        List<LootItem> possibleItems = items.FindAll(item => item.rarity == rolledRarity);
        
        if (possibleItems.Count == 0) return null;

        return possibleItems[Random.Range(0, possibleItems.Count)];
    }

    Rarity RollRarity()
    {
        float roll = Random.value;

        if (roll < 0.50f) return Rarity.Common;
        if (roll < 0.80f) return Rarity.Rare;
        if (roll < 0.95f) return Rarity.Epic;
        return Rarity.Legendary;
    }
}
