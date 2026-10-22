using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    [SerializeField] BattleUnit enemeyUnit;
    [SerializeField] BattleHud enemyHud;

    [SerializeField] BattleDialogueBox battleDialogueBox;
    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
{
    playerUnit.SetupCreature();
    playerHud.SetCreatureData(playerUnit.Creature);

    enemeyUnit.SetupCreature();

    yield return battleDialogueBox.SetDialogueText($"{enemeyUnit.Creature.Base.name} Virus has Appeared");
}
}
