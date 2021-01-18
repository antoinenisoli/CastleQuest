using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
class TileEffect_Yellow : TileEffect
{
    protected override StatType stat => StatType.Attack;
    public static TileType type => TileType.Attack;

    public override void Effect(List<Sc_Tile> tiles)
    {
        Sc_Player player = UnityEngine.Object.FindObjectOfType<Sc_Player>();
        player.GetAttack.Value += value + tiles[0].bonusValue;
        base.Effect(tiles);
    }
}
