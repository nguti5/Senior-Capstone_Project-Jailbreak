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

        private IEnumerator SetupBattle()
    {
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

    IEnumerator EnemyAction()
    {
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

        StartCoroutine(PlayerActionCoroutine());
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
        StartCoroutine(battleDialogueBox.SetDialogueText("Select Action"));
        yield return new WaitForSeconds (0.5f);
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

    IEnumerator selectAttack(int selectedAttack, List<Attack> attacks)
    {
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleDialogueText(true);

        var oldHPValue = enemyUnit.Creature.HP;
        
        if(selectedAttack == 0)
        {
            Debug.Log("Selected Attack 0");
            yield return battleDialogueBox.SetDialogueText($"You used {attacks[0].Base.Name}");
            var damageDesc = enemyUnit.Creature.RecieveDamage(playerUnit.Creature, attacks[0]);
            attacks[0].Mp--;
            //enemyHud.UpdateCreatureData(oldHPValue);
            yield return ShowDamageDescription(damageDesc);

            if (damageDesc.Dead)
            {
                yield return battleDialogueBox.SetDialogueText($"You deleted {enemyUnit.Creature.Base.name}");
                enemyUnit.playDeathAnimation();
                yield return new WaitForSeconds(1.5f);
                //IMPLEMNT WINNING SCENE TRANSITION ############################################################
            }

            else
            {
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(EnemyAction());
            }
        }
        else if(selectedAttack == 1)
        {
            Debug.Log("Selected Attack 1");
            yield return battleDialogueBox.SetDialogueText($"You used {attacks[1].Base.Name}");
            var damageDesc = enemyUnit.Creature.RecieveDamage(playerUnit.Creature, attacks[1]);
            attacks[1].Mp--;
            //enemyHud.UpdateCreatureData(oldHPValue);
            yield return ShowDamageDescription(damageDesc);

            if (damageDesc.Dead)
            {
                yield return battleDialogueBox.SetDialogueText($"You deleted {enemyUnit.Creature.Base.name}");
                enemyUnit.playDeathAnimation();
                yield return new WaitForSeconds(1.5f);
                //IMPLEMNT WINNING SCENE TRANSITION ############################################################
            }

            else
            {
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(EnemyAction());
            }            
        }
        else if(selectedAttack == 2)
        {
            Debug.Log("Selected Attack 2");
            yield return battleDialogueBox.SetDialogueText($"You used {attacks[2].Base.Name}");
            var damageDesc = enemyUnit.Creature.RecieveDamage(playerUnit.Creature, attacks[2]);
            attacks[2].Mp--;
            //enemyHud.UpdateCreatureData(oldHPValue);
            yield return ShowDamageDescription(damageDesc);

            if (damageDesc.Dead)
            {
                yield return battleDialogueBox.SetDialogueText($"You deleted {enemyUnit.Creature.Base.name}");
                enemyUnit.playDeathAnimation();
                yield return new WaitForSeconds(1.5f);
                //IMPLEMNT WINNING SCENE TRANSITION ############################################################
            }

            else
            {
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(EnemyAction());
            }            
        }
        else if(selectedAttack == 3)
        {
            Debug.Log("Selected Attack 3");
            yield return battleDialogueBox.SetDialogueText($"You used {attacks[3].Base.Name}");
            var damageDesc = enemyUnit.Creature.RecieveDamage(playerUnit.Creature, attacks[3]);
            attacks[3].Mp--;
            //enemyHud.UpdateCreatureData(oldHPValue);
            yield return ShowDamageDescription(damageDesc);

            if (damageDesc.Dead)
            {
                yield return battleDialogueBox.SetDialogueText($"You deleted {enemyUnit.Creature.Base.name}");
                enemyUnit.playDeathAnimation();
                yield return new WaitForSeconds(1.5f);
                //IMPLEMNT WINNING SCENE TRANSITION ############################################################
            }

            else
            {
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(EnemyAction());
            }
        }
        playerUnit.playAttackAnimation();
        enemyUnit.playRecieveAttackAnimation();
        yield return new WaitForSeconds(2.0f);
    }

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
