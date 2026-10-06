using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadMenu : MonoBehaviour
{
    public TMP_Text[] slotLabels;
    void Start(){
        for (int i = 0; i < slotLabels.Length; i++){
            slotLabels[i].text = PlayerPrefs.GetString("SlotName" + i, "Empty");
        }
    }

    public void Load(int slot){
        PlayerPrefs.SetInt("CurrentSlot", slot);

        if (PlayerPrefs.HasKey("SlotName" + slot)){
            SceneManager.LoadScene("Tutorial");
        }
        else{
            SceneManager.LoadScene("RenameField");
        } 
    }

    public void DeleteSlot(int slot){
        PlayerPrefs.DeleteKey("SlotName" + slot);
        PlayerPrefs.Save();
        slotLabels[slot].text = "Empty";
    }

    public void Back(){
        SceneManager.LoadScene("TitlePage");
    }
}