using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] Text dialogueText;

    public bool isWriting = false;
    public float charactersPerSecond;

    public IEnumerator SetDialogueText(string message)
    {
        isWriting = true;
        dialogueText.text = "";
        foreach(var character in message)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }
    }
}
