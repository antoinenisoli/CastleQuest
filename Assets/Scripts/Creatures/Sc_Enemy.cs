using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sc_Enemy : Sc_Creature
{
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
