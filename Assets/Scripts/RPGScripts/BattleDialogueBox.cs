using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] Text dialogueText;


    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject attackSelect;
    [SerializeField] private GameObject attackDescription;
    [SerializeField] private List<Button> actionTexts;
    [SerializeField] private List<Text> attackTexts;
    [SerializeField] private List<GameObject> attackButtons;
    [SerializeField] private Text mpText;
    [SerializeField] private Text typeText;
    [SerializeField] private BattleUnit playerUnit;


    void Update()
    {
        setAttackDescription(playerUnit.Creature.Attacks);
    }



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
        yield return new WaitForSeconds(0.5f);
    }

    public void ToggleDialogueText(bool activated)
    {
        dialogueText.enabled = activated;
    }

    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }

    public void ToggleAttacks(bool activated)
    {
        attackSelect.SetActive(activated);
        attackDescription.SetActive(activated);
    }

    public void SetCreatureAttacks(List<Attack> attacks)
    {
        for(int i = 0; i < attackTexts.Count; i++)
        {
            if(i < attacks.Count)
            {
                attackTexts[i].text = attacks[i].Base.Name;
                attackButtons[i].SetActive(true);
            }
            else
            {
                attackTexts[i].text = "N/A";
                attackButtons[i].SetActive(false);
            }
        }
    }

    public void setAttackDescription(List<Attack>attacks)
    {
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        if(selectedButton == attackButtons[0])
        {
            mpText.text = $"{attacks[0].Mp} / {attacks[0].Base.MP}";
            typeText.text = $"{attacks[0].Base.Type}";
        }
        else if(selectedButton == attackButtons[1])
        {
            mpText.text = $"{attacks[1].Mp} / {attacks[1].Base.MP}";
            typeText.text = $"{attacks[1].Base.Type}";;
        }
        else if(selectedButton == attackButtons[2])
        {
            mpText.text = $"{attacks[2].Mp} / {attacks[2].Base.MP}";
            typeText.text = $"{attacks[2].Base.Type}";
        }
        else if(selectedButton == attackButtons[3])
        {
            mpText.text = $"{attacks[3].Mp} / {attacks[3].Base.MP}";
            typeText.text = $"{attacks[3].Base.Type}";
        }
    }
}
