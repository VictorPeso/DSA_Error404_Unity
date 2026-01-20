using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoader] Nombre de escena vacío!");
            return;
        }

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetCurrentScore();
        }

        SceneManager.LoadScene(sceneName);
    }

    public void LoadLevel1()
    {
        LoadScene("Level_1");
    }

    public void LoadLevel2()
    {
        LoadScene("Level_2");
    }

    public void LoadLevel3()
    {
        LoadScene("Level_3");
    }

    public void LoadLevel4()
    {
        LoadScene("Level_4");
    }

    public void LoadLevel5()
    {
        LoadScene("Level_5");
    }

    public void BackToLevelSelector()
    {
        LoadScene("LevelSelector");
    }
}
