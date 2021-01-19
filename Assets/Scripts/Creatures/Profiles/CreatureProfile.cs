using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Profile", menuName = "Creature/Profiles")]
public class CreatureProfile : ScriptableObject
{
    public new string name = "Creature";
    public GameObject visualObject;
    public Sprite portrait;
    public Statistic[] myStats = new Statistic[4];
}
