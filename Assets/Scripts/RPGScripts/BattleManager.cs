using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;

    [SerializeField] BattleDialogueBox battleDialogueBox;

    [SerializeField] int defendRestoreCount = 2;
    [SerializeField] int defendRestoreAmount = 2;
    [SerializeField] float fleeChance = 0.5f;
    [SerializeField] List<ItemBase> battleItems;

    private List<BattleItemEntry> currentItems = new List<BattleItemEntry>();

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }
    
    public void BacktoActions() //small x button to return to action select.
    {
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleItems(false);
        battleDialogueBox.ToggleActions(true);
    }

    //begins coroutines for initalized battle scene, intializes units and huds. also decides inital turn order.
        private IEnumerator SetupBattle()
    {
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleItems(false);
        battleDialogueBox.ToggleDialogueText(true);
        
        
        playerUnit.SetupCreature();
        playerHud.SetCreatureData(playerUnit.Creature);

        battleDialogueBox.SetCreatureAttacks(playerUnit.Creature.Attacks);
        
        if(BattleTransition.EnemyBase != null)
        {
            enemyUnit._base = BattleTransition.EnemyBase;
            enemyUnit._level = BattleTransition.EnemyLevel;
        }

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

    //enemy turn actions. --------------------------------------------------------------------------------------------------
    
    IEnumerator EnemyAction()
    {
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleItems(false);
        battleDialogueBox.ToggleDialogueText(true);
        
        
        Attack attack = enemyUnit.Creature.RandomAttack();
        yield return battleDialogueBox.SetDialogueText($"{enemyUnit.Creature.Base.name} used {attack.Base.name}");
        
        var oldHPValue = playerUnit.Creature.HP;
        
        enemyUnit.playAttackAnimation();
        playerUnit.playRecieveAttackAnimation();

        var damageDesc = playerUnit.Creature.RecieveDamage(enemyUnit.Creature, attack);
        playerUnit.Creature.IsDefending = false;
        playerHud.UpdateCreatureData(oldHPValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Dead)
        {
            yield return battleDialogueBox.SetDialogueText($"You have been beaten by {enemyUnit.Creature.Base.name}");
            playerUnit.playDeathAnimation();
            yield return new WaitForSeconds(1.5f);
            BattleTransition.EndBattle(BattleResult.Lost);
        }

        else
        {
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerActionCoroutine());
        }

        yield return new WaitForSeconds(2.0f);
    }

    //Player Actions -------------------------------------------------------------------------------------------------
    public void PlayerAction()
    {
        StartCoroutine(PlayerActionCoroutine());
    }

    public void PlayerItem()
    {
        StartCoroutine(PlayerItemCoroutine());
    }
    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCoroutine());
    }

    public void PlayerDefend()
    {
        StartCoroutine(PlayerDefendCoroutine());
    }

    public void PlayerRun()
    {
        StartCoroutine(PlayerRunCoroutine());
    }
    
    IEnumerator PlayerActionCoroutine()
    {
        battleDialogueBox.ToggleAttacks(false);   
        battleDialogueBox.ToggleItems(false);  
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

    IEnumerator PlayerDefendCoroutine()
    {
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleDialogueText(true);

        var creature = playerUnit.Creature;
        creature.IsDefending = true;

        yield return battleDialogueBox.SetDialogueText($"{creature.Base.CreatureName} is defending!");

        var restored = creature.RestoreRandomAttackMP(defendRestoreCount, defendRestoreAmount);
        if(restored.Count > 0)
        {
            string names = string.Join(" and ", restored.ConvertAll(a => a.Base.Name));
            yield return battleDialogueBox.SetDialogueText($"Restored MP to {names}!");
        }
        else
        {
            yield return battleDialogueBox.SetDialogueText("MP is already full!");
        }
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(EnemyAction());
    }

    IEnumerator PlayerRunCoroutine()
    {
        battleDialogueBox.ToggleActions(false);
        battleDialogueBox.ToggleAttacks(false);
        battleDialogueBox.ToggleDialogueText(true);

        if (!BattleTransition.CanFlee)
        {
            yield return battleDialogueBox.SetDialogueText("You Cannot Flee From this Battle!");
            StartCoroutine(PlayerActionCoroutine());
            yield break;
        }

        bool escaped = playerUnit.Creature.Speed >= enemyUnit.Creature.Speed || UnityEngine.Random.value < fleeChance;

        if (escaped)
        {
            yield return battleDialogueBox.SetDialogueText("You got away safely!");
            yield return new WaitForSeconds(0.5f);
            BattleTransition.EndBattle(BattleResult.Fled);
        }
        else
        {
            yield return battleDialogueBox.SetDialogueText("Couldn't get away!");
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(EnemyAction());
        }
    }

    public void attackButtonPressed(int selectedAttack)
    {
        if(playerUnit.Creature.Attacks[selectedAttack].Mp <= 0)
        {
            return;
        }    
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
            BattleTransition.EndBattle(BattleResult.Won);
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
        if (desc.Defended)
        {
            yield return battleDialogueBox.SetDialogueText("The Attack was Blocked!");
        }
        
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

    IEnumerator PlayerItemCoroutine()
    {
        battleDialogueBox.ToggleActions(false);
        currentItems = GetUsableItems();

        if (currentItems.Count == 0)
        {
            battleDialogueBox.ToggleDialogueText(true);
            yield return battleDialogueBox.SetDialogueText("You Don't Have any Items.");
            StartCoroutine(PlayerActionCoroutine());
            yield break;
        }

        battleDialogueBox.ToggleDialogueText(false);
        battleDialogueBox.SetItems(currentItems);
        yield return new WaitForSeconds(0.2f);
        battleDialogueBox.ToggleItems(true);
    }

    private List<BattleItemEntry> GetUsableItems()
    {
        var list = new List<BattleItemEntry>();
        var catalog = LegacyCatalogManager.Instance;
        if(catalog == null || !catalog.IsInventoryLoaded) return list;

        foreach(var item in battleItems)
        {
            int count = catalog.GetItemCount(item.ItemId);
            if(count > 0) list.Add(new BattleItemEntry(item,count));
            if(list.Count == 4) break;
        }
        return list;
    }

    public void itemButtonPressed(int index)
    {
        if (index < 0 || index >= currentItems.Count) return;
        StartCoroutine(UseItem(currentItems[index].Item));
    }

    IEnumerator UseItem(ItemBase item)
    {
        battleDialogueBox.ToggleItems(false);
        battleDialogueBox.ToggleDialogueText(true);

        var creature = playerUnit.Creature;
        if(creature.HP >= creature.MaxHP)
        {
            yield return battleDialogueBox.SetDialogueText("HP is already full!");
            StartCoroutine(PlayerActionCoroutine());
            yield break;
        }

        yield return battleDialogueBox.SetDialogueText($"You used {item.DisplayName}!");
    
        bool? consumed = null;
        LegacyCatalogManager.Instance.ConsumeItem(item.ItemId, ok => consumed = ok);
        yield return new WaitUntil(() => consumed.HasValue);

        if(consumed == false)
        {
            yield return battleDialogueBox.SetDialogueText("But it failed...");
            StartCoroutine(PlayerActionCoroutine());
            yield break;
        }

        int oldHP = creature.HP;
        int healed = creature.Heal(item.HealAmount);
        playerHud.UpdateCreatureData(oldHP);

        yield return battleDialogueBox.SetDialogueText($"{creature.Base.CreatureName} recovered {healed} HP!");
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(EnemyAction());
    
    
    }


}


