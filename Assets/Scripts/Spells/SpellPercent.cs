using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPercent : Spell
{
    public override SpellType spellType => throw new System.NotImplementedException();
    [SerializeField] int bonusValue = 2;
    [Range(0, 100)] [SerializeField] protected int percentageValue = 50;
    protected List<Sc_Tile> selectedTiles = new List<Sc_Tile>();
    [SerializeField] protected StatType typeToReplace;
    [SerializeField] protected StatType newType;

    public override void Effect()
    {
        base.Effect();
        List<Sc_Tile> allTiles = new List<Sc_Tile>();
        foreach (var item in tileManager.grid)
        {
            if (item.GetComponent<Sc_Tile>().myTileEffect.stat == typeToReplace)
                allTiles.Add(item.GetComponent<Sc_Tile>());
        }

        int percent = allTiles.Count * percentageValue / 100;
        List<Sc_Tile> selectedTiles = new List<Sc_Tile>();
        for (int i = 0; i < percent; i++)
        {
            selectedTiles.Add(allTiles[i]);
        }

        foreach (var item in selectedTiles)
        {
            item.Creation();
            item.SetBonus(bonusValue);
        }
    }
}
