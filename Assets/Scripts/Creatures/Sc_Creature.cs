using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Creature : MonoBehaviour
{
    public Statistic GetLife => allPoints[StatType.Life];
    public Statistic GetMana => allPoints[StatType.Mana];
    public Statistic GetAttack => allPoints[StatType.Attack];
    public Statistic GetDefense => allPoints[StatType.Defense];

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
