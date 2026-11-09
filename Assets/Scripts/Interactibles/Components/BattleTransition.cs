using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleResult{Won,Lost,Fled}
public static class BattleTransition
{
    private const string BattleSceneName = "BattleScene1";

    public static CreatureBase EnemyBase { get; private set; }
    public static int EnemyLevel { get; private set; }
    public static bool InBattle { get; private set; }

    public static bool CanFlee {get; private set;} = true;

    private static Scene overworldScene;
    private static readonly List<GameObject> hiddenObjects = new List<GameObject>();
    private static Action<BattleResult> onBattleEnded;

    private static CursorLockMode savedLockMode;
    private static bool savedCursorVisible;

    public static void StartBattle(CreatureBase enemy, int level, Action<BattleResult> onEnded = null, bool canFlee = true)
    {
        if (InBattle) return; 

        if (!Application.CanStreamedLevelBeLoaded(BattleSceneName))
        {
            Debug.LogError($"Scene '{BattleSceneName}' isn't in the Build Profiles scene list.");
            return;
        }

        InBattle = true;
        EnemyBase = enemy;
        EnemyLevel = level;
        onBattleEnded = onEnded;
        CanFlee = canFlee;
        overworldScene = SceneManager.GetActiveScene();

        var op = SceneManager.LoadSceneAsync(BattleSceneName, LoadSceneMode.Additive);
        op.completed += _ =>
        {
            // Freeze the overworld: switch off every top-level object that's currently on
            hiddenObjects.Clear();
            foreach (var root in overworldScene.GetRootGameObjects())
            {
                if (root.activeSelf)
                {
                    hiddenObjects.Add(root);
                    root.SetActive(false);
                }
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(BattleSceneName));

            
            savedLockMode = Cursor.lockState;
            savedCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        };
    }

    public static void EndBattle(BattleResult result)
    {
        if (!InBattle) return;

        var op = SceneManager.UnloadSceneAsync(BattleSceneName);
        op.completed += _ =>
        {
            // Switch the overworld back on exactly as it was
            foreach (var obj in hiddenObjects)
                if (obj != null) obj.SetActive(true);
            hiddenObjects.Clear();

            SceneManager.SetActiveScene(overworldScene);
            Cursor.lockState = savedLockMode;
            Cursor.visible = savedCursorVisible;

            InBattle = false;
            EnemyBase = null;
            CanFlee = true;

            var callback = onBattleEnded;
            onBattleEnded = null;
            callback?.Invoke(result);
        };
    }
}