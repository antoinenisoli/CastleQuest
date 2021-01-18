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

    public TileEffect_Green(float value) : base(value)
    {
        this.value = value;
    }
}
