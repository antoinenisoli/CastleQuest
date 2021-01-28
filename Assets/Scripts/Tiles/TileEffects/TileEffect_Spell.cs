using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEffect_Spell
{
    protected StatType stat;
    protected Sc_GameManager gameManager;
    protected Sc_Player player;
    [SerializeField] protected int value;

    public abstract void Effect(List<Sc_Tile> tiles);
}
