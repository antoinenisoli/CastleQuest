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
    protected abstract StatType stat { get; }
    public float value = 1;

    public virtual void Effect(List<Sc_Tile> tiles)
    {
        Sc_EventManager.instance.onGrowStat.Invoke(stat);
    }
}
