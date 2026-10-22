using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public CreatureBase _base;
    public int _level;
    public bool _isPlayer;
    public Creature Creature{get;set;}

    private Image creatureImage;

    private void Awake()
    {
        creatureImage = GetComponent<Image>();

    }

    public void SetupCreature()
    {
        Creature = new Creature(_base, _level);
    }
}
