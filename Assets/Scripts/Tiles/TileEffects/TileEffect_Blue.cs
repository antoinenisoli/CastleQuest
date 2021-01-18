using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
class TileEffect_Blue : TileEffect
{
    protected override StatType stat => StatType.MP;
    public static TileType type => TileType.Mana;

    public override void Effect(List<Sc_Tile> tiles)
    {
        Sc_Player player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        player.GetMana.Value += value;
        base.Effect(tiles);
    }
}
