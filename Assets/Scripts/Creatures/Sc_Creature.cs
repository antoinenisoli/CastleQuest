using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureType
{
    Player,
    Mob,
}

public class Sc_Creature : MonoBehaviour
{
    public Statistic GetLife => allPoints[StatType.PV];
    public Statistic GetMana => allPoints[StatType.MANA];
    public Statistic GetAttack => allPoints[StatType.Attack];
    public Statistic GetDefense => allPoints[StatType.Defense];

    public new string name = "Creature";    
    [SerializeField] Statistic[] myPoints = new Statistic[4];
    Dictionary<StatType, Statistic> allPoints = new Dictionary<StatType, Statistic>();
    bool isDead;

    private void Awake()
    {
        foreach (var item in myPoints)
        {
            if (!allPoints.ContainsKey(item.myStat))
                allPoints.Add(item.myStat, item);
        }

        GetLife.Initialise();
        GetMana.Initialise();
    }

    public Statistic GetState(StatType statName)
    {
        return allPoints[statName];
    }

    public void Attack(Sc_Creature target)
    {
        float computeDamages = GetAttack.Value - target.GetDefense.Value;
        target.GetLife.ModifyValue(-computeDamages);
    }

    public void Death()
    {
        isDead = true;
    }
}
