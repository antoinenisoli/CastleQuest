using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CreatureType
{
    Player,
    Mob,
}

public abstract class Sc_Creature : MonoBehaviour
{
    public Statistic GetLife => allPoints[StatType.HP];
    public Statistic GetMana => allPoints[StatType.MP];
    public Statistic GetAttack => allPoints[StatType.Attack];
    public Statistic GetDefense => allPoints[StatType.Defense];

    public CreatureProfile profile;
    [SerializeField] protected float animSpeed = 0.4f;
    [SerializeField] Statistic[] myPoints = new Statistic[4];
    Dictionary<StatType, Statistic> allPoints = new Dictionary<StatType, Statistic>();
    public bool isDead;
    Animator anim;
    protected Vector3 basePos;

    private void Awake()
    {
        GameObject newSprite = Instantiate(profile.sprite, transform.position, transform.rotation, transform);
        anim = newSprite.GetComponent<Animator>();
        basePos = transform.position;

        for (int i = 0; i < myPoints.Length; i++)
        {
            myPoints[i].MaxValue = profile.myStats[i].MaxValue;
            myPoints[i].Value = profile.myStats[i].Value;
        }

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

    public virtual void StartAttack(Sc_Creature target)
    {              
        StartCoroutine(Attack(target));
    }

    IEnumerator Attack(Sc_Creature target)
    {
        yield return new WaitForSeconds(1);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.1f);
        float computeDamages = GetAttack.Value - target.GetDefense.Value;
        target.ModifyHealth(-computeDamages);
        target.transform.DOShakePosition(0.1f, computeDamages / 30);
        Sc_EventManager.instance.onUpdateStats.Invoke();
        yield return new WaitForSeconds(1);
        transform.DOMoveX(basePos.x, animSpeed);
    }

    public void ModifyHealth(float amount)
    {
        GetLife.ModifyValue(amount);
        if (GetLife.Value <= 0)
            Death();

        anim.SetBool("isDead", isDead);
    }

    public virtual void Death()
    {
        isDead = true;
        print(gameObject + " is dead");
    }
}
