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
    [SerializeField] protected Statistic[] myPoints = new Statistic[4];
    protected Dictionary<StatType, Statistic> allPoints = new Dictionary<StatType, Statistic>();
    public bool isDead;
    Animator anim;
    protected Vector3 basePos;

    public virtual void Awake()
    {
        GameObject newSprite = Instantiate(profile.visualObject, transform.position, transform.rotation, transform);
        anim = newSprite.GetComponent<Animator>();
        basePos = transform.position;
        foreach (var item in myPoints)
        {
            if (!allPoints.ContainsKey(item.myStat))
                allPoints.Add(item.myStat, item);
        }

        GetLife.Initialise();
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
        if (computeDamages > 0)
        {
            target.ModifyHealth(-computeDamages);
            target.transform.DOShakePosition(0.1f, 0.8f);
        }
        else
            target.transform.DOShakePosition(0.1f, 0.15f);

        Sc_EventManager.instance.onUpdateStats.Invoke();
        yield return new WaitForSeconds(1);
        transform.DOMoveX(basePos.x, animSpeed);
    }

    public void ModifyHealth(float amount)
    {
        GetLife.ModifyValue(amount);
        if (GetLife.Value <= 0)
            Death();
    }

    public virtual void Death()
    {
        isDead = true;
        anim.SetBool("isDead", isDead);
    }
}
