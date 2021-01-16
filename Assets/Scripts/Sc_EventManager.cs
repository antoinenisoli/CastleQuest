using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sc_EventManager : MonoBehaviour
{
    public static Sc_EventManager instance;
    public UnityEvent onUpdateStats;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

   
}
