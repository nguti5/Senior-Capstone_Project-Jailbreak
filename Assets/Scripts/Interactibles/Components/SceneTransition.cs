
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    Level,
    Battle
}

public static class SceneTransition
{
    public static string PendingSpawnId { get; private set; }

    public static TransitionType PendingTransitionType { get; private set; }
        = TransitionType.Level;

    public static void Load(
        string sceneName,
        string spawnId = null,
        TransitionType transitionType = TransitionType.Level)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"Scene '{sceneName}' can't be loaded. " +
                "Is it added to File > Build Profiles > Scene List?"
            );
            return;
        }

        PendingSpawnId = spawnId;
        PendingTransitionType = transitionType;

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.BeginTransition(sceneName);
        }
        else
        {
            Debug.LogWarning(
                "SceneTransitionManager not found. Loading scene normally."
            );

            SceneManager.LoadScene(sceneName);
        }
    }

    public static void ClearPendingSpawn()
    {
        PendingSpawnId = null;
    }
}
