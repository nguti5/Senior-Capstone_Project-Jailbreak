using UnityEngine;

public class RevealOnLoadIfComplete : MonoBehaviour
{
    [SerializeField] private string objectiveId = "l1_destroy_scam";
    [SerializeField] private GameObject target;   // the hidden cat

    private void Start()
    {
        if (target != null && SaveManager.IsComplete(objectiveId))
            target.SetActive(true);
    }
}
