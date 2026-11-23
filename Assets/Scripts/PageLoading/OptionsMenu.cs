using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    private void Start()
    {
        GameAudio.PinOptions(true);
    }

    public void Back()
    {
        SceneManager.LoadScene("TitlePage");
    }
}
