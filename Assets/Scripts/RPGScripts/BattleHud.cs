using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour 
{
    public Text creatureName;
    public Text creatureLevel;
    
    public HealthBar healthBar;

    public Text creatureHealth;

    private Creature _creature;
    public void SetCreatureData(Creature creature)
    {
        _creature = creature;
        creatureName.text = creature.Base.CreatureName;
        creatureLevel.text = $"Lv: {creature.Level}";
        UpdateCreatureData(creature.HP);
    }

    public void UpdateCreatureData(int oldHPValue)
    {
        StartCoroutine(healthBar.SetSmoothHP((float)_creature.HP/_creature.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPValue));
    }

    IEnumerator DecreaseHealthPoints(int oldHPValue)
    {
        while(oldHPValue > _creature.HP)
        {
            oldHPValue--;
            creatureHealth.text = $"{oldHPValue}/{_creature.MaxHP}";
            yield return new WaitForSeconds(0.05f);

        }

        creatureHealth.text = $"{_creature.HP}/{_creature.MaxHP}";
    }
}
