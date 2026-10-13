using UnityEngine;
using UnityEngine.SceneManagement;
 

public static class SceneTransition
{
    public static string PendingSpawnId { get; private set; }
 
    public static void Load(string sceneName, string spawnId = null)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' can't be loaded. Is it added to File > Build Profiles > Scene List?");
            return;
        }
 
        PendingSpawnId = spawnId;
        SceneManager.LoadScene(sceneName);
    }
 
    public static void ClearPendingSpawn() => PendingSpawnId = null;
}
