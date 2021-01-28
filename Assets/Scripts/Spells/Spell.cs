using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpellType
{
    None,
    Ice,
    Fire,
    Shield,
}

public abstract class Spell : MonoBehaviour
{
    public abstract SpellType spellType { get; }
    protected Sc_TileManager tileManager;
    protected Sc_GameManager gameManager;
    [SerializeField] protected float manaCost;
    protected Sc_Player player;
    Button myButton;
    Text costText;

    public void Awake()
    {
        costText = GetComponentInChildren<Text>();
        player = FindObjectOfType<Sc_Player>();
        gameManager = FindObjectOfType<Sc_GameManager>();
        tileManager = FindObjectOfType<Sc_TileManager>();
        myButton = GetComponent<Button>();
    }

    public virtual void Start()
    {
        costText.text = manaCost + "";
        Sc_EventManager.instance.onUpdateStats.AddListener(UpdateUI);
        UpdateUI();
    }

    public virtual void Effect()
    {
        player.GetMana.Value -= manaCost;
        Sc_EventManager.instance.onSpellInvocation.Invoke(spellType);
        Sc_EventManager.instance.onUpdateStats.Invoke();
    }

    public void UpdateUI()
    {
        myButton.interactable = (player.GetMana.Value - manaCost) >= 0 && gameManager.canPlay && tileManager.canSwap;
    }
}
