using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndGamePage : MonoBehaviour
{
    [SerializeField] private float showSeconds = 3f;
    [SerializeField] private float minSecondsBeforeSkip = 2f;   // so a leftover key press can't skip it instantly
    [SerializeField] private bool quitWhenDone = false;         // false = go back to the title page
    [SerializeField] private string titleScene = "TitlePage";

    private IEnumerator Start()
    {
        SaveManager.MarkComplete("game_complete");
        Debug.Log("Player has completed the game.");

        float timer = 0f;
        while (timer < showSeconds)
        {
            timer += Time.unscaledDeltaTime;

            if (timer > minSecondsBeforeSkip && PressedAnything())
                break;

            yield return null;
        }

        Finish();
    }

    private bool PressedAnything()
    {
        bool key = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool click = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        return key || click;
    }

    private void Finish()
    {
        if (quitWhenDone)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;   // Application.Quit does nothing in the editor
#else
            Application.Quit();
#endif
        }
        else
        {
            SceneManager.LoadScene(titleScene);
        }
    }
}