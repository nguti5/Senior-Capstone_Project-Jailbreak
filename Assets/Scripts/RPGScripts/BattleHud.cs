using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour 
{
    public Text creatureName;
    public Text creatureLevel;
    
    public HealthBar healthBar;
    public void SetCreatureData(Creature creature)
    {
        
        creatureName.text = creature.Base.CreatureName;
        creatureLevel.text = $"Lv: {creature.Level}";
        healthBar.SetHP(creature.HP/creature.MaxHP);
    }
}
