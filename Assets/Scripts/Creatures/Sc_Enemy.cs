using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sc_Enemy : Sc_Creature
{
    EnemyProfile myProfile => profile as EnemyProfile;

    public override void Awake()
    {
        base.Awake();
        for (int i = 1; i < myProfile.statsPoints + 1; i++)
        {
            int random = Random.Range((int)StatType.Attack, (int)StatType.Defense + 1);
            myPoints[random].Value++;
        }
    }

    public override void StartAttack(Sc_Creature target)
    {
        transform.DOMoveX(basePos.x - 1, animSpeed);
        base.StartAttack(target);
    }

    public override void Death()
    {
        base.Death();
        Sc_EventManager.instance.onWin.Invoke(true);
    }
}
