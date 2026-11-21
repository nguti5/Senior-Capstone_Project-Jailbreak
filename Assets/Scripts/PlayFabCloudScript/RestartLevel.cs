using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour 
{
    [Tooltip("Every saved progress id starting with one of these is erased, e.g. l1_")]
    [SerializeField] private string[] progressPrefixes = { "l1_" };

    [Tooltip("Inventory items from this level that get taken away, e.g. mailroom_key")]
    [SerializeField] private string[] itemsToRemove;

    [SerializeField] private float delay = 0.5f;

    [Header("Where to restart")]
    [Tooltip("Scene to load after a loss. Leave empty to reload this room.")]
    [SerializeField] private string sceneToLoad;

    [Tooltip("Optional SpawnPoint id in that scene. Leave empty to use its default spawn.")]
    [SerializeField] private string spawnId;

    private bool restarting;

    public void Restart()   // call from a BattleEncounter's On Player Lost
    {
        if (restarting) return;
        restarting = true;
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSecondsRealtime(delay);

        SaveManager.ClearProgress(progressPrefixes);

        // wait for the items to be removed, or the reloaded room would still see them
        if (itemsToRemove != null && itemsToRemove.Length > 0 && LegacyCatalogManager.Instance != null)
        {
            bool done = false;
            LegacyCatalogManager.Instance.RevokeItems(itemsToRemove, _ => done = true);
            while (!done) yield return null;
        }

        string target = string.IsNullOrEmpty(sceneToLoad) ? SceneManager.GetActiveScene().name : sceneToLoad;

        SceneTransition.Load(target, spawnId);
    }
}