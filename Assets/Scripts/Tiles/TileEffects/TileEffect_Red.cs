using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[Serializable]
class TileEffect_Red : TileEffect
{
    protected override StatType stat => StatType.HP;
    public static TileType type => TileType.Life;

    public TileEffect_Red(float value) : base(value)
    {
        this.value = value;
    }

    public override void Effect(List<Sc_Tile> tiles)
    {
        Sc_Player player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        player.ModifyHealth(value);
        base.Effect(tiles);
    }
}
