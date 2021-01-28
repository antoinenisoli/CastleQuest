using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sc_Player : Sc_Creature
{
    PlayerProfile myProfile => profile as PlayerProfile;

    public override void Awake()
    {
        base.Awake();
        ResetStats();
    }

    public void ResetStats()
    {
        GetAttack.Value = myProfile.attack.Value;
        GetDefense.Value = myProfile.defense.Value;
    }

    public override void StartAttack(Sc_Creature target)
    {
        transform.DOMoveX(basePos.x + 1, animSpeed);
        base.StartAttack(target);
    }

    public override void Death()
    {
        base.Death();
        Sc_EventManager.instance.onWin.Invoke(false);
    }
}
