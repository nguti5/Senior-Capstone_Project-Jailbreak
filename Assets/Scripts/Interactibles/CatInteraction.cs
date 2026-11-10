
using System.Collections;
using UnityEngine;

public class CatInteraction : MonoBehaviour, IInteractible
{
    [SerializeField] private string catName = "Peenut";

    [Header("Name Tag UI")]
    [SerializeField] private GameObject catNameTagPanel;
    [SerializeField] private float displayDuration = 0.5f;

    [Header("Objective")]
    [Tooltip("Objective checked off when player reads the name tag.")]
    [SerializeField] private string objectiveId = "find_password";

    private Coroutine hideCoroutine;

    private void Start()
    {
        if (catNameTagPanel != null)
            catNameTagPanel.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log("The name tag reads: " + catName);

        if (!string.IsNullOrEmpty(objectiveId) && ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.Complete(objectiveId);

        if (catNameTagPanel == null)
            return;

        catNameTagPanel.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideNameTag());
    }

    private IEnumerator HideNameTag()
    {
        yield return new WaitForSeconds(displayDuration);

        catNameTagPanel.SetActive(false);
        hideCoroutine = null;
    }
}
