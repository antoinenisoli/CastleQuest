using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class TileEffect_Mana : TileEffect
{
    public override StatType stat => StatType.MP;
    public static StatType type => StatType.MP;
}
