using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProfile", menuName = "Creature/EnemyProfiles")]
public class EnemyProfile : CreatureProfile
{
    [Header("Enemy")]
    public int statsPoints = 5;
}
