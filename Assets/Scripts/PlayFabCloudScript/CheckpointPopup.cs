using System.Collections;
using UnityEngine;

public class CheckpointPopup : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private float showSeconds = 2.5f;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        SaveManager.CheckpointSaved += Show;
    }

    private void OnDisable()
    {
        SaveManager.CheckpointSaved -= Show;
    }

    private void Show()
    {
        Debug.Log("Checkpoint saved: showing panel");
        StopAllCoroutines();
        StartCoroutine(ShowThenHide());
    }

    private IEnumerator ShowThenHide()
    {
        panel.SetActive(true);
        yield return new WaitForSecondsRealtime(showSeconds);
        panel.SetActive(false);
    }
}