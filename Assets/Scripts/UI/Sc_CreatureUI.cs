using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
            statValueText.text = creature.GetState(thisStat).Value + "/" + creature.GetState(thisStat).MaxValue;
            if (statBar != null)
            {
                statBar.maxValue = creature.GetState(thisStat).MaxValue;
                statBar.value = creature.GetState(thisStat).Value;
            }
        }
    }

    [SerializeField] Sc_Creature myCreature;
    [SerializeField] Text creatureName;
    [SerializeField] Image displayCreaturePortrait;
    [SerializeField] Sprite creaturePortrait;

    [Header("Grow Text")]
    [SerializeField] float growStrength = 0.3f;
    [SerializeField] float growDuration = 0.3f;

    [Header("Stats")]
    [SerializeField] Text attackValue;
    [SerializeField] Text defenseValue;
    [SerializeField] StatInfo[] statsInfos = new StatInfo[2];
    Dictionary<StatType, StatInfo> myInfos = new Dictionary<StatType, StatInfo>();

    private void Start()
    {
        Sc_EventManager.instance.onUpdateStats.AddListener(SetInfo);
        displayCreaturePortrait.sprite = creaturePortrait;
        if (myCreature.GetComponent<Sc_Player>())
            Sc_EventManager.instance.onGrowStat.AddListener(GrowStat);

        SetInfo();
        foreach (var info in statsInfos)
        {
            myInfos.Add(info.thisStat, info);
        }
    }

    public void GrowStat(StatType stat)
    {
        Transform statText = null;
        if (myInfos.ContainsKey(stat))
            statText = myInfos[stat].statValueText.transform;
        else
        {
            switch (stat)
            {
                case StatType.Attack:
                    statText = attackValue.transform;
                    break;

                case StatType.Defense:
                    statText = defenseValue.transform;
                    break;
            }
        }

        Vector3 baseScale = statText.localScale;
        statText.DOKill();
        statText.DOScale(baseScale * growStrength, growDuration);
        statText.DOScale(baseScale, growDuration/2).SetDelay(growDuration);
    }

    public void SetInfo()
    {
        foreach (var item in statsInfos)
        {
            item.UpdateDisplay(myCreature);
        }

        creatureName.text = myCreature.name;
        attackValue.text = myCreature.GetAttack.Value + "";
        defenseValue.text = myCreature.GetDefense.Value + "";
    }
}
