using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[Serializable]
public abstract class TileEffect
{
    protected virtual StatType stat { get; }

    protected float value = 1;
    protected Sc_Player player;

    public TileEffect(float value)
    {
        this.value = value;
    }

    public virtual void Effect(List<Sc_Tile> tiles)
    {
        player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        int bonus = 0;
        foreach (var item in tiles)
        {
            if (item.bonusValue > bonus)
                bonus = item.bonusValue;
        }

        player.GetState(stat).Value += value + bonus;
        Sc_EventManager.instance.onGrowStat.Invoke(stat);
    }
}
