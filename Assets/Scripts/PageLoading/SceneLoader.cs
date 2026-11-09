using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadWithLoadingScreen(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // call this on Unity Events
    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}