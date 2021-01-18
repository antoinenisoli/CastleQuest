using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_EventManager : MonoBehaviour
{
    public static Sc_EventManager instance;
    public class BoolEvent : UnityEvent<bool> { };
    public class StatEvent : UnityEvent<StatType> { };
    public class SpellEvent : UnityEvent<SpellType> { };

    public BoolEvent onWin = new BoolEvent();
    public UnityEvent onUpdateStats = new UnityEvent();
    public StatEvent onGrowStat = new StatEvent();
    public SpellEvent onSpellInvocation = new SpellEvent();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
