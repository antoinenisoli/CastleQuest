using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[Serializable]
class TileEffect_Life : TileEffect
{
    public override StatType stat => StatType.HP;
    public static StatType type => StatType.HP;

    public override void Effect(List<Sc_Tile> tiles)
    {
        Sc_Player player = FindObjectOfType<Sc_Player>();
        player.ModifyHealth(value);
        base.Effect(tiles);
    }
}
