using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NameEntry : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void ConfirmName()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot");
        PlayerPrefs.SetString("SlotName" + slot, nameInput.text);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Tutorial");
    }
}