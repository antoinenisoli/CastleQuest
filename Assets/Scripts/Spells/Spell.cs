using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Spell : MonoBehaviour
{
    [SerializeField] protected float manaCost;
    protected Sc_Player player;
    Button myButton;

    public void Awake()
    {
        player = FindObjectOfType<Sc_Player>();
        myButton = GetComponent<Button>();
    }

    public virtual void Effect()
    {
        player.GetMana.Value -= manaCost;
        myButton.interactable = (player.GetMana.Value - manaCost) > 0;
        Sc_EventManager.instance.onUpdateStats.Invoke();
    }
}
