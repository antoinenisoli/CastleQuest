using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellShield : Spell
{
    Sc_TileManager tileManager;

    private void Start()
    {
        tileManager = FindObjectOfType<Sc_TileManager>();
    }

    public override void Effect()
    {
        base.Effect();
        List<Sc_Tile> allTiles = new List<Sc_Tile>();
        foreach (var item in tileManager.grid)
        {
            if (item.GetComponent<Sc_Tile>().myType == TileType.Attack)
                allTiles.Add(item.GetComponent<Sc_Tile>());
        }

        int percent = allTiles.Count * 50 / 100;
        print(percent);

        List<Sc_Tile> selectedTiles = new List<Sc_Tile>();
        for (int i = 0; i < percent; i++)
        {
            selectedTiles.Add(allTiles[i]);
        }

        foreach (var item in selectedTiles)
        {
            item.bonusValue = 2;
            item.Creation((int)TileType.Defense);
        }
    }
}
