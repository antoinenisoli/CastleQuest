using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellIce : Spell
{
    public override SpellType spellType => SpellType.Ice;
    [SerializeField] int count = 4;

    public override void Effect()
    {
        base.Effect();
        List<Sc_Tile> allTiles = new List<Sc_Tile>();
        foreach (var item in tileManager.grid)
            allTiles.Add(item.GetComponent<Sc_Tile>());

        List<Sc_Tile> selectedTiles = new List<Sc_Tile>();
        for (int i = 0; i < count; i++)
        {
            bool loop = true;
            while (loop)
            {
                int random = Random.Range(0, allTiles.Count);
                if (!selectedTiles.Contains(allTiles[random]))
                {
                    selectedTiles.Add(allTiles[random]);
                    loop = false;
                }
            }
            
        }

        foreach (var item in selectedTiles)
        {
            item.Creation();
            item.SetEffect(spellType);
        }
    }
}
