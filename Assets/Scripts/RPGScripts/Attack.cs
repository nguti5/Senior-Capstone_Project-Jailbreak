using UnityEngine;

public class Attack
{
    private AttackBase _base;
    private int _mp;
    public AttackBase Base
    {
        get => _base;
        set => _base = value; 
    }

    public int Mp
    {
        get => _mp;
        set => _mp = value;
    }

    public Attack(AttackBase aBase)
    {
        Base = aBase;
        Mp = aBase.MP;
    }
}
