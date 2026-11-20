using System.Collections;
using UnityEngine;

public class RevealWhenComplete : MonoBehaviour
{
    [SerializeField] private string objectiveId;
    [SerializeField] private GameObject target;      // the hidden object
    [SerializeField] private float delay = 1.8f;

    private IEnumerator Start()
    {
        if (target == null)
        {
            Debug.LogError("RevealWhenComplete: Target isn't assigned!");
            yield break;
        }

        Debug.Log($"RevealWhenComplete watching '{objectiveId}' (target '{target.name}')");

        bool alreadyDone = IsDone();
        target.SetActive(alreadyDone);
        if (alreadyDone) yield break;

        while (!IsDone())
            yield return new WaitForSeconds(0.25f);

        yield return new WaitForSeconds(delay);
        Debug.Log("RevealWhenComplete: objective done, showing " + target.name);
        target.SetActive(true);
    }

    private bool IsDone()
    {
        if (SaveManager.IsComplete(objectiveId)) return true;
        return ObjectiveManager.Instance != null && ObjectiveManager.Instance.IsObjectiveDone(objectiveId);
    }
}