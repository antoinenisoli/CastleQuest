using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_CreatureUI : MonoBehaviour
{
    [Serializable]
    class StatInfo
    {
        public StatType thisStat;
        public Text statValueText;
        public Slider statBar;

        public void UpdateDisplay(Sc_Creature creature)
        {
            statValueText.text = creature.name + " " + creature.GetState(thisStat).Value + " / " + creature.GetState(thisStat).MaxValue;
            if (statBar != null)
            {
                statBar.maxValue = creature.GetState(thisStat).MaxValue;
                statBar.value = creature.GetState(thisStat).Value;
            }
        }
    }

    [SerializeField] Sc_Creature myCreature;
    [SerializeField] Text statName;
    [SerializeField] StatInfo[] statsInfos = new StatInfo[2];
    [SerializeField] Text attackValue, defenseValue;

    private void Start()
    {
        Sc_EventManager.instance.onUpdateStats.AddListener(SetInfo);
        SetInfo();
    }

    public void SetInfo()
    {
        foreach (var item in statsInfos)
        {
            item.UpdateDisplay(myCreature);
        }

        statName.text = myCreature.name;
        attackValue.text = myCreature.GetAttack.Value + "";
        defenseValue.text = myCreature.GetDefense.Value + "";
    }
}
