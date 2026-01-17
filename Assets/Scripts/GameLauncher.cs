using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script que se ejecuta al inicio del juego
/// - Lee el username desde Android (via Intent)
/// - Configura el APIManager
/// - Descarga el perfil del jugador
/// </summary>
public class GameLauncher : MonoBehaviour
{
    [Header("Usuario de prueba (para Unity Editor)")]
    [Tooltip("Usuario usado cuando se ejecuta desde el Editor de Unity")]
    public string testUsername = "testuser";

    void Start()
    {
        string username = testUsername; // Valor por defecto para Unity Editor

        // Si estamos en Android real (no en el Editor)
        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");

            if (intent.Call<bool>("hasExtra", "USERNAME"))
            {
                username = intent.Call<string>("getStringExtra", "USERNAME");
                Debug.Log($"[GameLauncher] ✅ Username recibido de Android: {username}");
            }
            else
            {
                Debug.LogWarning("[GameLauncher] ⚠️ No se recibió USERNAME de Android, usando default");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameLauncher] ❌ Error leyendo Intent de Android: {e.Message}");
        }
        #else
        Debug.Log($"[GameLauncher] 🎮 Modo Editor - Usando username de prueba: {username}");
        #endif

        // Configurar el username en el APIManager
        if (APIManager.Instance != null)
        {
            APIManager.Instance.SetUsername(username);

            // Descargar perfil del usuario (objetos comprados, stats, etc)
            StartCoroutine(APIManager.Instance.GetProfile(OnProfileLoaded));
        }
        else
        {
            Debug.LogError("[GameLauncher] ❌ APIManager no encontrado en la escena!");
            Debug.LogError("[GameLauncher] Asegúrate de tener un GameObject con el script APIManager");
        }
    }

    /// <summary>
    /// Callback cuando se descarga el perfil del backend
    /// </summary>
    void OnProfileLoaded(string jsonProfile)
    {
        Debug.Log("[GameLauncher] 📦 Perfil cargado del backend");
        Debug.Log($"[GameLauncher] JSON recibido: {jsonProfile}");

        try
        {
            // 1. Parsear JSON completo usando UnityProfileDTO
            UnityProfileResponse profile = JsonUtility.FromJson<UnityProfileResponse>(jsonProfile);

            if (profile == null || profile.user == null)
            {
                Debug.LogError("[GameLauncher] ❌ Error: JSON parseado es null");
                return;
            }

            // 2. Cargar progreso en ProgressManager
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.LoadFromProfile(profile);
                Debug.Log($"[GameLauncher] ✅ ActFrag cargado: {profile.user.actFrag}");
                Debug.Log($"[GameLauncher] ✅ BestScore: {profile.user.bestScore}");
                Debug.Log($"[GameLauncher] ✅ Monedas: {profile.user.monedas}");
            }
            else
            {
                Debug.LogError("[GameLauncher] ❌ ProgressManager no encontrado en la escena");
                Debug.LogError("[GameLauncher] Asegúrate de tener un GameObject con ProgressManager.cs");
            }

            // 3. Cargar inventario en EquipmentManager
            if (EquipmentManager.Instance != null)
            {
                EquipmentManager.Instance.LoadInventoryFromBackend(profile.objects);
                Debug.Log($"[GameLauncher] ✅ Inventario cargado: {profile.objects?.Length ?? 0} objetos");
            }
            else
            {
                Debug.LogWarning("[GameLauncher] ⚠️ EquipmentManager no encontrado en la escena");
            }

            // 4. Ir a selector de niveles
            Debug.Log("[GameLauncher] 🎮 Cargando selector de niveles...");
            SceneManager.LoadScene("LevelSelector");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameLauncher] ❌ Error parseando JSON: {e.Message}");
            Debug.LogError($"[GameLauncher] Stack: {e.StackTrace}");
        }
    }
}
