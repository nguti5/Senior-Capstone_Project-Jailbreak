using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;

    [SerializeField] BattleDialogueBox battleDialogueBox;
    private void Start()
    {
        StartCoroutine(SetupBattle());
    }
    
    public void BacktoActions() //small x button to return to action select.
    {
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleActions(true);
    }

    //begins coroutines for initalized battle scene, intializes units and huds. also decides inital turn order.
        private IEnumerator SetupBattle()
    {
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleDialogueText(true);
        
        
        playerUnit.SetupCreature();
        playerHud.SetCreatureData(playerUnit.Creature);

        battleDialogueBox.SetCreatureAttacks(playerUnit.Creature.Attacks);
        
        enemyUnit.SetupCreature();

        yield return battleDialogueBox.SetDialogueText($"{enemyUnit.Creature.Base.name} Virus has Appeared");
        
        if(playerUnit.Creature.Speed < enemyUnit.Creature.Speed)
        {
            StartCoroutine(EnemyAction());
        }
        else
        {
            PlayerAction();
        }

        
        
    }

    //enemy turn actions.
    
    IEnumerator EnemyAction()
    {
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleDialogueText(true);
        
        
        Attack attack = enemyUnit.Creature.RandomAttack();
        yield return battleDialogueBox.SetDialogueText($"{enemyUnit.Creature.Base.name} used {attack.Base.name}");
        
        var oldHPValue = playerUnit.Creature.HP;
        
        enemyUnit.playAttackAnimation();
        playerUnit.playRecieveAttackAnimation();

        var damageDesc = playerUnit.Creature.RecieveDamage(enemyUnit.Creature, attack);
        playerHud.UpdateCreatureData(oldHPValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Dead)
        {
            yield return battleDialogueBox.SetDialogueText($"You have been beaten by {enemyUnit.Creature.Base.name}");
            playerUnit.playDeathAnimation();
            yield return new WaitForSeconds(1.5f);
            //Implement Game Over Screen Here!!!#################################################################
        }

        else
        {
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerActionCoroutine());
        }

        yield return new WaitForSeconds(2.0f);
    }
    public void PlayerAction()
    {
        StartCoroutine(PlayerActionCoroutine());
    }


    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCoroutine());
    }
    
    IEnumerator PlayerActionCoroutine()
    {
        battleDialogueBox.ToggleAttacks(false);   
        StartCoroutine(battleDialogueBox.SetDialogueText("Select Action"));
        yield return new WaitForSeconds(1.5f);
        battleDialogueBox.ToggleDialogueText(false);   
        battleDialogueBox.ToggleActions(true);
    }

    IEnumerator PlayerAttackCoroutine()
    {
        battleDialogueBox.ToggleDialogueText(false);
        battleDialogueBox.ToggleActions(false);
        yield return new WaitForSeconds (0.2f);
        battleDialogueBox.ToggleAttacks(true);
    }

    public void attackButtonPressed(int selectedAttack)
    {
        StartCoroutine(selectAttack(selectedAttack, playerUnit.Creature.Attacks));
    }

    //Function to that allows player to select 1 of 4 attacks after selecting the action button.

    IEnumerator selectAttack(int selectedAttack, List<Attack> attacks)
    {
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleDialogueText(true);

        var attack = attacks[selectedAttack];
        var oldHPValue = enemyUnit.Creature.HP;

        yield return battleDialogueBox.SetDialogueText($"You used {attack.Base.Name}");

        
        playerUnit.playAttackAnimation();
        enemyUnit.playRecieveAttackAnimation();

        var damageDesc = enemyUnit.Creature.RecieveDamage(playerUnit.Creature, attack);
        attack.Mp--;
        //enemyHud.UpdateCreatureData(oldHPValue);

        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Dead)
        {
            yield return battleDialogueBox.SetDialogueText($"You deleted {enemyUnit.Creature.Base.name}");
            enemyUnit.playDeathAnimation();
            yield return new WaitForSeconds(1.5f);
            // IMPLEMENT WINNING SCENE TRANSITION
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(EnemyAction());
        }
    }

    //if certain damage parameters (such as type effectiveness and critical hits) proc then display unique message
    IEnumerator ShowDamageDescription(DamageDescription desc)
    {
        if(desc.Critical > 1)
        {
            yield return battleDialogueBox.SetDialogueText("It's a Critical Hit!");
        }

        if(desc.Type > 1)
        {
            yield return battleDialogueBox.SetDialogueText("It's Extermely Effective!");
        }

        if(desc.Type < 1)
        {
            yield return battleDialogueBox.SetDialogueText("It's not Effective...");
        }
    }
}


