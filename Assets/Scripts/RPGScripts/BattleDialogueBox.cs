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
    [SerializeField] private Image typeImage;
    [SerializeField] private List<TypeIcon> typeIcons;
    [Serializable] public struct TypeIcon
    {
        public CreatureType type;
        public Sprite sprite;
    }

    private Dictionary<CreatureType, Sprite> iconLookup;

    void Awake()
    {
        iconLookup = new Dictionary<CreatureType,Sprite>();
        foreach(var entry in typeIcons)
        {
            iconLookup[entry.type] = entry.sprite;
        }

        ClearAttackInfo();
    }


    void Update()
    {
    if (shownIndex >= 0) ShowAttackInfo(shownIndex);
    }


    private int shownIndex = -1;
    public bool isWriting = false;
    public float charactersPerSecond;
 
    //function that writes out dialogue box text a character at a time.
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

    //toggles visibility of dialogue box text
    public void ToggleDialogueText(bool activated)
    {
        dialogueText.enabled = activated;
    }
    
    //toggles visibility of action buttons

    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }
    //toggles visibility of attack buttons
    public void ToggleAttacks(bool activated)
    {
        ClearAttackInfo();   
        attackSelect.SetActive(activated);
        attackDescription.SetActive(activated);
    }

    //function to set the names for the attack buttons.
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

    //function that sets up side window on what the attack's typing and remaining MP is.

    public void ShowAttackInfo(int index)
    {
        var attacks = playerUnit.Creature?.Attacks;
        if (attacks == null || index < 0 || index >= attacks.Count) return;

        shownIndex = index;
        var attack = attacks[index];

        mpText.text = $"{attack.Mp} / {attack.Base.MP}";
        typeText.text = $"{attack.Base.Type}";

        if (iconLookup.TryGetValue(attack.Base.Type, out var icon) && icon != null)
        {
            typeImage.sprite = icon;
            typeImage.enabled = true;
        }
        else
        {
            typeImage.enabled = false;   // no sprite for this type yet
        }
    }

        private void ClearAttackInfo() //helper function that hides element icon when not in use.
    {
        shownIndex = -1;
        typeImage.sprite = null;
        typeImage.enabled = false;
        mpText.text = "";
        typeText.text = "";
    }
}
