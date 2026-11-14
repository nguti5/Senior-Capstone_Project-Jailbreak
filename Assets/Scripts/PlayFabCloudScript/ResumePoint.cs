using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumePoint : MonoBehaviour
{
    [Header("Popup (optional)")]
    [SerializeField] private GameObject panel;
    [SerializeField] private float delay = 1f;
    [SerializeField] private float showSeconds = 3f;

    private IEnumerator Start()
    {
        string scene = SceneManager.GetActiveScene().name;
        Debug.Log("ResumePoint started in " + scene);

        if (panel != null) panel.SetActive(false);

        SaveManager.SetResumeScene(scene);

        if (panel == null || !SaveManager.HasSave(SaveManager.CurrentSlot)) yield break;

        yield return new WaitForSecondsRealtime(delay);
        panel.SetActive(true);
        yield return new WaitForSecondsRealtime(showSeconds);
        panel.SetActive(false);
    }
}