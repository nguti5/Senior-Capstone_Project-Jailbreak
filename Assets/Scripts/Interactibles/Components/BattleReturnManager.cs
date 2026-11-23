
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BattleReturnManager
{
    public static string ReturnSceneName { get; private set; }
    public static string ReturnSpawnId { get; private set; }

    public static bool HasReturnLocation =>
        !string.IsNullOrEmpty(ReturnSceneName);

    public static void SaveReturnLocation(string spawnId)
    {
        ReturnSceneName = SceneManager.GetActiveScene().name;
        ReturnSpawnId = spawnId;

        Debug.Log(
            $"Battle return location saved: " +
            $"{ReturnSceneName}, spawn: {ReturnSpawnId}"
        );
    }

    public static void ReturnFromBattle()
    {
        if (!HasReturnLocation)
        {
            Debug.LogError(
                "No battle return location was saved!"
            );
            return;
        }

        string sceneName = ReturnSceneName;
        string spawnId = ReturnSpawnId;

        ReturnSceneName = null;
        ReturnSpawnId = null;

        SceneTransition.Load(
            sceneName,
            spawnId,
            TransitionType.Level
        );
    }
}
