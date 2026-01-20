using UnityEngine;

/// <summary>
/// Script de debug para verificar configuración del LevelManager
/// Añade este script a cualquier GameObject en la escena y mira la consola
/// </summary>
public class LevelManagerDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("========== LEVEL MANAGER DEBUG ==========");
        
        // Verificar si existe LevelManager
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("[DEBUG] ❌ NO SE ENCONTRÓ LevelManager en la escena!");
            Debug.LogError("[DEBUG] SOLUCIÓN: Crea un GameObject vacío llamado 'LevelManager' y añade el script LevelManager.cs");
            return;
        }
        
        Debug.Log("[DEBUG] ✓ LevelManager encontrado");
        Debug.Log($"[DEBUG] Level Number: {levelManager.levelNumber}");
        
        // Verificar boss
        if (levelManager.boss == null)
        {
            Debug.LogError("[DEBUG] ❌ Boss NO está asignado en el LevelManager!");
            Debug.LogError("[DEBUG] SOLUCIÓN: En el Inspector del LevelManager, arrastra el GameObject del boss al campo 'Boss'");
        }
        else
        {
            Debug.Log($"[DEBUG] ✓ Boss asignado: {levelManager.boss.name}");
            
            // Verificar si el boss tiene Enemy.cs
            Enemy enemyScript = levelManager.boss.GetComponent<Enemy>();
            if (enemyScript == null)
            {
                Debug.LogError($"[DEBUG] ❌ El boss '{levelManager.boss.name}' NO tiene componente Enemy.cs!");
                Debug.LogError("[DEBUG] SOLUCIÓN: Añade el script Enemy.cs al GameObject del boss");
            }
            else
            {
                Debug.Log("[DEBUG] ✓ Boss tiene componente Enemy.cs");
                
                // Verificar listeners del evento OnDeath
                int listenerCount = enemyScript.OnDeath.GetPersistentEventCount();
                Debug.Log($"[DEBUG] Listeners en OnDeath: {listenerCount}");
            }
        }
        
        // Verificar VictoryPanel
        if (levelManager.victoryPanel == null)
        {
            Debug.LogError("[DEBUG] ❌ VictoryPanel NO está asignado en el LevelManager!");
            Debug.LogError("[DEBUG] SOLUCIÓN: En el Inspector del LevelManager, arrastra el VictoryPanel al campo 'Victory Panel'");
        }
        else
        {
            Debug.Log($"[DEBUG] ✓ VictoryPanel asignado: {levelManager.victoryPanel.name}");
            
            // Verificar si tiene el script VictoryPanel.cs
            VictoryPanel panelScript = levelManager.victoryPanel.GetComponent<VictoryPanel>();
            if (panelScript == null)
            {
                Debug.LogError($"[DEBUG] ❌ El VictoryPanel '{levelManager.victoryPanel.name}' NO tiene componente VictoryPanel.cs!");
                Debug.LogError("[DEBUG] SOLUCIÓN: Añade el script VictoryPanel.cs al GameObject del panel");
            }
            else
            {
                Debug.Log("[DEBUG] ✓ VictoryPanel tiene componente VictoryPanel.cs");
            }
            
            // Verificar si está activo
            if (levelManager.victoryPanel.activeSelf)
            {
                Debug.LogWarning("[DEBUG] ⚠ VictoryPanel está ACTIVO al inicio (debería estar desactivado)");
            }
            else
            {
                Debug.Log("[DEBUG] ✓ VictoryPanel está desactivado (correcto)");
            }
        }
        
        // Verificar ProgressManager
        if (ProgressManager.Instance == null)
        {
            Debug.LogError("[DEBUG] ❌ ProgressManager.Instance es NULL!");
            Debug.LogError("[DEBUG] SOLUCIÓN: Asegúrate de que hay un GameObject con ProgressManager.cs en la escena InitScene");
        }
        else
        {
            Debug.Log("[DEBUG] ✓ ProgressManager encontrado");
        }
        
        // Verificar EquipmentManager
        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("[DEBUG] ❌ EquipmentManager.Instance es NULL!");
            Debug.LogError("[DEBUG] SOLUCIÓN: Asegúrate de que hay un GameObject con EquipmentManager.cs en la escena InitScene");
        }
        else
        {
            Debug.Log("[DEBUG] ✓ EquipmentManager encontrado");
        }
        
        // Verificar APIManager
        if (APIManager.Instance == null)
        {
            Debug.LogError("[DEBUG] ❌ APIManager.Instance es NULL!");
            Debug.LogError("[DEBUG] SOLUCIÓN: Asegúrate de que hay un GameObject con APIManager.cs en la escena InitScene");
        }
        else
        {
            Debug.Log("[DEBUG] ✓ APIManager encontrado");
        }
        
        Debug.Log("========== FIN DEBUG ==========");
    }
}
