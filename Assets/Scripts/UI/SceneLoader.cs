using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script simple para cargar escenas
/// Úsalo en los botones del LevelSelector configurando OnClick manualmente
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Carga una escena por nombre
    /// Configura este método en el OnClick del botón en Inspector
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoader] ❌ Nombre de escena vacío!");
            return;
        }

        Debug.Log($"[SceneLoader] Cargando escena: {sceneName}");

        // Resetear score si existe ProgressManager
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetCurrentScore();
        }

        // Cargar escena
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Carga Level_1
    /// </summary>
    public void LoadLevel1()
    {
        LoadScene("Level_1");
    }

    /// <summary>
    /// Carga Level_2
    /// </summary>
    public void LoadLevel2()
    {
        LoadScene("Level_2");
    }

    /// <summary>
    /// Carga Level_3
    /// </summary>
    public void LoadLevel3()
    {
        LoadScene("Level_3");
    }

    /// <summary>
    /// Carga Level_4
    /// </summary>
    public void LoadLevel4()
    {
        LoadScene("Level_4");
    }

    /// <summary>
    /// Carga Level_5
    /// </summary>
    public void LoadLevel5()
    {
        LoadScene("Level_5");
    }

    /// <summary>
    /// Vuelve al LevelSelector
    /// </summary>
    public void BackToLevelSelector()
    {
        LoadScene("LevelSelector");
    }
}
