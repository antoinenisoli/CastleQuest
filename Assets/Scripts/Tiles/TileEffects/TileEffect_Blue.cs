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

    public TileEffect_Blue(float value) : base(value)
    {
        this.value = value;
    }
}
