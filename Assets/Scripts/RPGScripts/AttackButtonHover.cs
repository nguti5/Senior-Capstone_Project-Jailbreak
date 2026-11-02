using UnityEngine;
using UnityEngine.EventSystems;

public class AttackButtonHover : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private int attackIndex;              // 0-3, set per button in the Inspector
    [SerializeField] private BattleDialogueBox dialogueBox;

    public void OnPointerEnter(PointerEventData eventData) => dialogueBox.ShowAttackInfo(attackIndex);
    public void OnSelect(BaseEventData eventData) => dialogueBox.ShowAttackInfo(attackIndex); 
}