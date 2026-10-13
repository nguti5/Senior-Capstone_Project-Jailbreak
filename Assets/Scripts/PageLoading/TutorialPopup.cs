using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text popupText;

    [TextArea(2, 5)]
    public string[] steps;

    int current = 0;
    string doneKey;

    void Start(){
        doneKey = "TutorialDone" + PlayerPrefs.GetInt("CurrentSlot");

        // skip the tutorial for saves that already finished it
        if (PlayerPrefs.GetInt(doneKey, 0) == 1 || steps.Length == 0){
            popupPanel.SetActive(false);
            return;
        }

        ShowStep(0);
    }

    void Update(){
    if (!popupPanel.activeSelf) return;

    if (Keyboard.current == null) return;

    if (Keyboard.current.enterKey.wasPressedThisFrame ||
        Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        NextStep();
    }

    void ShowStep(int index){
        popupPanel.SetActive(true);
        popupText.text = steps[index] + "\n\n<size=60%>Press Enter to continue</size>";
    }

    void NextStep(){
        current++;
        if (current >= steps.Length)
            EndTutorial();
        else
            ShowStep(current);
    }

    void EndTutorial(){
        popupPanel.SetActive(false);
        PlayerPrefs.SetInt(doneKey, 1);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneLoader.LoadWithLoadingScreen("Battle-Tutorial");
    }
}