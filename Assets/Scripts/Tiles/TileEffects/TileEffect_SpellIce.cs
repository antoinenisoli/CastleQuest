using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileEffect_SpellIce : TileEffect_Spell
{
    public static SpellType type => SpellType.Ice;

    public TileEffect_SpellIce(float value) : base(value)
    {
        this.value = value;
        player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        gameManager = UnityEngine.Object.FindObjectOfType<Sc_GameManager>();
    }

    public override void Effect(List<Sc_Tile> tiles)
    {
        player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        gameManager.ChangeAction((int)value);
    }
}
