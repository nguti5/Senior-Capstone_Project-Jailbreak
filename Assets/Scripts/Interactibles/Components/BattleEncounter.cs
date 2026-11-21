
using UnityEngine;
using UnityEngine.Events;

public class BattleEncounter : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private CreatureBase enemyCreature;
    [SerializeField] private int enemyLevel = 5;

    [Header("Behavior")]
    [Tooltip("Once a player wins, the encounter can't be started again.")]
    [SerializeField] private bool oneTimeOnly = true;
    [SerializeField] private bool canFlee = true;

    [Header("Results")]
    [SerializeField] private UnityEvent onPlayerWon;
    [SerializeField] private UnityEvent onPlayerLost;
    [SerializeField] private UnityEvent onPlayerFled;

    private bool defeated;
    private bool battleStarting;

    public void StartBattle()
    {
        if (oneTimeOnly && defeated)
            return;

        if (battleStarting || BattleTransition.InBattle)
            return;

        battleStarting = true;

        if (SceneTransitionManager.Instance != null)
        {
            if (SceneTransitionManager.Instance.IsTransitioning)
            {
                battleStarting = false;
                return;
            }

            SceneTransitionManager.Instance.BeginBattleTransition(
                StartBattleInternal
            );
        }
        else
        {
            StartBattleInternal();
        }
    }

    private void StartBattleInternal()
    {
        battleStarting = false;

        BattleTransition.StartBattle(
            enemyCreature,
            enemyLevel,
            OnBattleFinished,
            canFlee
        );
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
