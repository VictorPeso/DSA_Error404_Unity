using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Gestor de comunicación con el backend REST API
/// Singleton que persiste entre escenas
/// </summary>
public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

    // ⚠️ IMPORTANTE: Cambia esta IP por la de tu PC
    // Para obtenerla: Abre CMD y escribe "ipconfig" (busca IPv4 Address)
    // Ejemplo: 192.168.1.100, 192.168.0.15, etc.
    [Header("Configuración del Backend")]
    [Tooltip("IP de tu PC donde corre el backend Java")]
    public string serverIP = "localhost";
    public int serverPort = 8080;

    private string baseURL;
    private string currentUsername;

    void Awake()
    {
        // Singleton: Este objeto sobrevive entre escenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Construir URL base
        baseURL = $"http://{serverIP}:{serverPort}/dsaApp";
        Debug.Log($"[API] URL del backend configurada: {baseURL}");
    }

    /// <summary>
    /// Configurar el username del jugador actual
    /// </summary>
    public void SetUsername(string username)
    {
        currentUsername = username;
        Debug.Log($"[API] Username configurado: {username}");
    }

    public string GetUsername()
    {
        return currentUsername;
    }

    // ==================== ENDPOINTS ====================

    /// <summary>
    /// 1. AÑADIR MONEDAS - Se llama cuando el jugador mata enemigos
    /// </summary>
    public IEnumerator AddCoins(int amount)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogError("[API] No hay username configurado. No se pueden añadir monedas.");
            yield break;
        }

        string url = baseURL + "/game/unity/add-coins";
        string jsonBody = $"{{\"username\":\"{currentUsername}\",\"amount\":{amount}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"[API] Enviando {amount} monedas al servidor...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[API] ✅ Monedas añadidas correctamente. Respuesta: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"[API] ❌ Error al añadir monedas: {request.error}");
                Debug.LogError($"[API] Código de respuesta: {request.responseCode}");
            }
        }
    }

    /// <summary>
    /// 2. OBTENER PERFIL - Descarga datos del usuario (monedas, objetos, progreso)
    /// </summary>
    public IEnumerator GetProfile(System.Action<string> onSuccess = null)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogError("[API] No hay username configurado. No se puede obtener perfil.");
            yield break;
        }

        string url = baseURL + "/game/unity/profile/" + currentUsername;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            Debug.Log($"[API] Descargando perfil del servidor para: {currentUsername}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"[API] ✅ Perfil descargado correctamente");
                Debug.Log($"[API] JSON: {jsonResponse}");

                if (onSuccess != null)
                {
                    onSuccess(jsonResponse);
                }
            }
            else
            {
                Debug.LogError($"[API] ❌ Error al obtener perfil: {request.error}");
                Debug.LogError($"[API] Código de respuesta: {request.responseCode}");
            }
        }
    }

    /// <summary>
    /// 3. ACTUALIZAR PROGRESO - Guarda ActFrag y BestScore en el backend
    /// </summary>
    public IEnumerator UpdateProgress(int actFrag, int bestScore)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogError("[API] No hay username configurado. No se puede actualizar progreso.");
            yield break;
        }

        string url = baseURL + "/game/unity/update-progress";
        string jsonBody = $"{{\"username\":\"{currentUsername}\",\"actFrag\":{actFrag},\"bestScore\":{bestScore}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"[API] Actualizando progreso: ActFrag={actFrag}, BestScore={bestScore}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[API] ✅ Progreso actualizado correctamente");
            }
            else
            {
                Debug.LogError($"[API] ❌ Error al actualizar progreso: {request.error}");
            }
        }
    }

    /// <summary>
    /// 4. BOSS LOOT - Añade un objeto gratis al inventario (drop de jefe)
    /// </summary>
    public IEnumerator AddBossLoot(string objectId)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogError("[API] No hay username configurado. No se puede añadir loot.");
            yield break;
        }

        string url = baseURL + "/game/unity/boss-loot";
        string jsonBody = $"{{\"nombre\":\"{currentUsername}\",\"objectId\":\"{objectId}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"[API] Añadiendo loot del jefe: {objectId}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[API] ✅ Loot añadido correctamente: {objectId}");
            }
            else
            {
                Debug.LogError($"[API] ❌ Error al añadir loot: {request.error}");
                Debug.LogError($"[API] Código de respuesta: {request.responseCode}");
            }
        }
    }
}
