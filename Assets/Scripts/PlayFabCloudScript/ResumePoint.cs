using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumePoint : MonoBehaviour
{
    void Start()
    {
        Debug.Log("ResumePoint started in " + SceneManager.GetActiveScene().name);
        SaveManager.SetResumeScene(SceneManager.GetActiveScene().name);
    }
}