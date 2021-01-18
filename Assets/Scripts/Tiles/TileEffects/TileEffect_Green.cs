using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
class TileEffect_Green : TileEffect
{
    protected override StatType stat => StatType.Defense;
    public static TileType type => TileType.Defense;

    public override void Effect(List<Sc_Tile> tiles)
    {
        Sc_Player player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        player.GetDefense.Value += value + tiles[0].bonusValue;
        base.Effect(tiles);
    }
}
