using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName ="Creature/New Creature")]
public class CreatureBase : ScriptableObject
{
    [SerializeField] private int ID;
    [SerializeField] private string creatureName; public string CreatureName => creatureName;
    [TextArea][SerializeField] private string description; public string Description => description; 
    [SerializeField] private Sprite frontSprite; public Sprite FrontSprite => frontSprite;
    [SerializeField] private Sprite hurtSprite; public Sprite HurtSprite => hurtSprite;
    [SerializeField] private CreatureType type; public CreatureType Type => type;

    //Stats
    [SerializeField] private int maxHP; public int MaxHP => maxHP;
    [SerializeField] private int attack; public int Attack => attack;
    [SerializeField] private int defense; public int Defense => defense;
    [SerializeField] private int spAttack; public int SpAttack => spAttack;
    [SerializeField] private int spDefense; public int SpDefense => spDefense;
    [SerializeField] private int speed; public int Speed => speed;

    [SerializeField] private List<KnownAttack> knownAttacks;

    public List<KnownAttack> KnownAttacks => knownAttacks; 
    public enum CreatureType
    {
        normal,
        worm,
        trojan,
        bot,
        adware

    }


    [Serializable]
    public class KnownAttack
    {
        [SerializeField] private AttackBase attack;
        [SerializeField] private int level;
        public AttackBase Attack => attack;
        public int Level => level;
    }
}
