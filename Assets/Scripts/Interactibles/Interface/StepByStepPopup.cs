using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class StepByStepPopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject popupPanel;
    public TMP_Text popupText;

    [Header("Content")]
    [TextArea(2, 5)]
    public string[] steps;

    [Header("Behavior")]
    [Tooltip("Leave blank for the main tutorial. Give any extra tutorial a unique id.")]
    public string tutorialId = "";
    public bool playOnStart = true;
    public bool onlyOnce = true;

    [Header("Events")]
    public UnityEvent onComplete;

    int current;
    bool running;
    int startFrame;

    string DoneKey =>
        "TutorialDone" +
        (string.IsNullOrEmpty(tutorialId) ? "" : "_" + tutorialId) +
        PlayerPrefs.GetInt("CurrentSlot");

    void Start()
    {
        if (playOnStart) Play();
    }

    // Callable from buttons, triggers, other UnityEvents, or code
    public void Play()
    {
        if (running) return;

        bool alreadyDone = onlyOnce && PlayerPrefs.GetInt(DoneKey, 0) == 1;
        if (alreadyDone || steps.Length == 0)
        {
            onComplete?.Invoke();   // nothing to show, but the sequence still counts as finished
            return;
        }

        current = 0;
        running = true;
        startFrame = Time.frameCount;
        ShowStep(0);
    }

    void Update()
    {
        // skip the frame we started on, so the key press that started it can't also skip step 1
        if (!running || Time.frameCount == startFrame) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            NextStep();
    }

    void ShowStep(int index)
    {
        popupPanel.SetActive(true);
        popupText.text = steps[index] + "\n\n<size=60%>Press Enter to continue</size>";
    }

    void NextStep()
    {
        current++;
        if (current >= steps.Length)
            Finish();
        else
            ShowStep(current);
    }

    void Finish()
    {
        running = false;
        popupPanel.SetActive(false);

        if (onlyOnce)
        {
            PlayerPrefs.SetInt(DoneKey, 1);
            PlayerPrefs.Save();
        }

        onComplete?.Invoke();
    }

    // handy for testing or a "replay tutorial" button
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(DoneKey);
    }
}