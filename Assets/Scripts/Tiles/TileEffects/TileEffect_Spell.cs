using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEffect_Spell : TileEffect
{
    protected override StatType stat => base.stat;
    protected Sc_GameManager gameManager;

    public TileEffect_Spell(float value) : base(value)
    {
        this.value = value;
        player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        gameManager = UnityEngine.Object.FindObjectOfType<Sc_GameManager>();
    }
}
