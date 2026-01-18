using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

    [Header("Configuración del Backend")]
    [Tooltip("IP del backend")]
    public string serverIP = "localhost";
    public int serverPort = 8080;

    private string baseURL;
    private string currentUsername;

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
            return;
        }

        baseURL = $"http://{serverIP}:{serverPort}/dsaApp";
    }

    public IEnumerator UpdateObjectQuantity(string objectId, int newQuantity, System.Action onSuccess = null)
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogError("[API] No hay username configurado. No se puede actualizar objeto.");
            yield break;
        }

        string url = baseURL + "/game/unity/update-object-quantity";
        string jsonBody = $"{{\"username\":\"{currentUsername}\",\"objectId\":\"{objectId}\",\"newQuantity\":{newQuantity}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[API] Cantidad de objeto {objectId} actualizada a {newQuantity}");
                if (onSuccess != null)
                {
                    onSuccess();
                }
            }
            else
            {
                Debug.LogError($"[API] Error al actualizar cantidad de objeto: {request.error}");
            }
        }
    }

    public void SetUsername(string username)
    {
        currentUsername = username;
    }

    public string GetUsername()
    {
        return currentUsername;
    }

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

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[API] Error al añadir monedas: {request.error}");
            }
        }
    }

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
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                if (onSuccess != null)
                {
                    onSuccess(jsonResponse);
                }
            }
            else
            {
                Debug.LogError($"[API] Error al obtener perfil: {request.error}");
            }
        }
    }

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

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[API] Error al actualizar progreso: {request.error}");
            }
        }
    }

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

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[API] Error al añadir loot: {request.error}");
            }
        }
    }
}
