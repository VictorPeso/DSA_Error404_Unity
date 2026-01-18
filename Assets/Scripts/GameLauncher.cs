using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour
{
    [Header("Usuario de prueba (para Unity Editor)")]
    [Tooltip("Usuario usado cuando se ejecuta desde el Editor de Unity")]
    public string testUsername = "testuser";

    void Start()
    {
        string username = testUsername;

        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");

            if (intent.Call<bool>("hasExtra", "USERNAME"))
            {
                username = intent.Call<string>("getStringExtra", "USERNAME");
            }
            else
            {
                Debug.LogWarning("[GameLauncher] No se recibió USERNAME de Android, usando default");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameLauncher] Error leyendo Intent de Android: {e.Message}");
        }
        #endif

        if (APIManager.Instance != null)
        {
            APIManager.Instance.SetUsername(username);
            StartCoroutine(APIManager.Instance.GetProfile(OnProfileLoaded));
        }
        else
        {
            Debug.LogError("[GameLauncher] APIManager no encontrado en la escena!");
        }
    }

    void OnProfileLoaded(string jsonProfile)
    {
        try
        {
            UnityProfileResponse profile = JsonUtility.FromJson<UnityProfileResponse>(jsonProfile);

            if (profile == null || profile.user == null)
            {
                Debug.LogError("[GameLauncher] Error: JSON parseado es null");
                return;
            }

            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.LoadFromProfile(profile);
            }
            else
            {
                Debug.LogError("[GameLauncher] ProgressManager no encontrado en la escena");
            }

            if (EquipmentManager.Instance != null)
            {
                EquipmentManager.Instance.LoadInventoryFromBackend(profile.objects);
            }
            else
            {
                Debug.LogWarning("[GameLauncher] EquipmentManager no encontrado en la escena");
            }

            SceneManager.LoadScene("LevelSelector");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameLauncher] Error parseando JSON: {e.Message}");
        }
    }
}
