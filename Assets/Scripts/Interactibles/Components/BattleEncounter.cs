using UnityEngine;
using UnityEngine.Events;

public class BattleEncounter : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private CreatureBase enemyCreature;
    [SerializeField] private int enemyLevel = 5;

    [Header("Behvaior")]
    [Tooltip("Once a Player Wins the Encounter can't be started again")]
    [SerializeField] private bool oneTimeOnly = true;
    [SerializeField] private bool canFlee = true;

    [Header("Results")]
    [SerializeField] private UnityEvent onPlayerWon;
    [SerializeField] private UnityEvent onPlayerLost;
    [SerializeField] private UnityEvent onPlayerFled;

    private bool defeated;

    public void StartBattle()
    {
        if(oneTimeOnly && defeated) return;
        BattleTransition.StartBattle(enemyCreature, enemyLevel, OnBattleFinished, canFlee);
    }

    private void OnBattleFinished(BattleResult result)
    {
        switch (result)
        {
            case BattleResult.Won:
                defeated = true;
                onPlayerWon?.Invoke();
                break;
            case BattleResult.Lost:
                onPlayerLost?.Invoke();
                break;
            case BattleResult.Fled:
                onPlayerFled?.Invoke();
                break;
        }
    }
}