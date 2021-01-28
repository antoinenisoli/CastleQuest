using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Creature/AllyProfiles")]
public class PlayerProfile : CreatureProfile
{
    [Header("Player")]
    public Statistic mana;
    public Statistic attack;
    public Statistic defense;
}
