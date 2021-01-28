using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureProfile : ScriptableObject
{
    public new string name = "Creature";
    public GameObject visualObject;
    public Sprite portrait;
    public Statistic life;
}
