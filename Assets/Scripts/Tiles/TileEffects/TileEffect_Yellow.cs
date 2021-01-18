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

    public TileEffect_Yellow(float value) : base(value)
    {
        this.value = value;
    }
}
