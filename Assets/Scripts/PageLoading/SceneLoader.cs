using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string NextScene = "TitlePage";
    public static float WaitSeconds = 15f;

    public static void LoadWithLoadingScreen(string sceneName, float wait = 10f){
        NextScene = sceneName;
        WaitSeconds = wait;
        SceneManager.LoadScene("BattleLoading");
    }
}