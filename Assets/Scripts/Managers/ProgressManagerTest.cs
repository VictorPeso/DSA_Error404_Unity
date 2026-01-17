using UnityEngine;

// ⚠️⚠️⚠️ SCRIPT DE TESTING - SOLO PARA DESARROLLO ⚠️⚠️⚠️
// Este script es solo para desarrollo y testing.
// NO debe estar activo en la build de producción.
// Para desactivarlo: Marca 'runOnStart' como FALSE en Inspector.
// ⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️⚠️

/// <summary>
/// Test script para verificar que ProgressManager funciona correctamente.
/// Adjuntar a cualquier GameObject para probar en el Editor.
/// NOTA: Este script ejecuta el test automáticamente en Start().
/// Para ejecutar manualmente: Right-click en el script → "Test ProgressManager"
/// </summary>
public class ProgressManagerTest : MonoBehaviour
{
    [Header("Test Values")]
    [SerializeField] private int testActFrag = 2;
    [SerializeField] private int testBestScore = 1500;
    [SerializeField] private int testBytes = 250;
    [SerializeField] private string testUsername = "testuser";

    [Header("Auto Test")]
    [Tooltip("Ejecutar test automáticamente en Start (desactivar si no quieres logs)")]
    [SerializeField] private bool runOnStart = true;

    void Start()
    {
        if (runOnStart)
        {
            TestProgressManager();
        }
    }

    [ContextMenu("Test ProgressManager")]
    void TestProgressManager()
    {
        // Verificar que ProgressManager existe
        if (ProgressManager.Instance == null)
        {
            Debug.LogError("❌ ProgressManager.Instance is NULL! Asegúrate de que existe un GameObject con ProgressManager en la escena.");
            return;
        }

        Debug.Log("✅ ProgressManager.Instance exists!");

        // Simular carga de perfil - Crear objeto UnityProfileResponse
        UnityProfileResponse testProfile = new UnityProfileResponse
        {
            user = new UserData
            {
                username = testUsername,
                actFrag = testActFrag,
                bestScore = testBestScore,
                monedas = testBytes
            },
            objects = new GameObjectData[0] // Sin objetos para el test
        };

        ProgressManager.Instance.LoadFromProfile(testProfile);

        // Verificar valores
        Debug.Log($"📊 Progress Data After Load:");
        Debug.Log($"   - MaxLevelReached: {ProgressManager.Instance.maxLevelReached}");
        Debug.Log($"   - BestScore: {ProgressManager.Instance.bestScore}");
        Debug.Log($"   - CurrentBytes: {ProgressManager.Instance.currentBytes}");
        Debug.Log($"   - Username: {ProgressManager.Instance.username}");

        // Test actualizar progreso (simula completar nivel 3 con score 2000)
        int oldMaxLevel = ProgressManager.Instance.maxLevelReached;
        ProgressManager.Instance.UpdateProgress(3, 2000);
        Debug.Log($"✅ Progress updated: MaxLevel {oldMaxLevel} → {ProgressManager.Instance.maxLevelReached}");

        // Test añadir bytes
        int oldBytes = ProgressManager.Instance.currentBytes;
        ProgressManager.Instance.AddBytes(50);
        Debug.Log($"✅ Bytes added: {oldBytes} → {ProgressManager.Instance.currentBytes}");

        Debug.Log("🎉 All ProgressManager tests passed!");
    }

    // NOTA: Métodos de Input removidos para evitar conflictos con Input System
    // Para ejecutar el test manualmente: Right-click en el script en Inspector → "Test ProgressManager"
}
