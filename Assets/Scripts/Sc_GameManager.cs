using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Sc_GameManager : MonoBehaviour
{
    Sc_Player mainPlayer;
    Sc_Enemy currentEnemy;
    [SerializeField] int maxActions = 15;
    int remainingActions;
    [SerializeField] Text displayActions;
    [SerializeField] CanvasGroup fightText;
    [SerializeField] GameObject endScreen;
    public bool canPlay = true;
    Image screen;
    Text txt;

    private void Awake()
    {
        mainPlayer = FindObjectOfType<Sc_Player>();
        currentEnemy = FindObjectOfType<Sc_Enemy>();
        remainingActions = maxActions;
        displayActions.text = remainingActions + "";
        fightText.DOFade(0, 0);

        screen = endScreen.GetComponentInChildren<Image>();
        txt = endScreen.GetComponentInChildren<Text>();
        endScreen.GetComponentInChildren<Image>().DOFade(0, 0);
        endScreen.GetComponentInChildren<Text>().DOFade(0, 0);
    }

    private void Start()
    {
        Sc_EventManager.instance.onWin.AddListener(GameOver);
        Sc_EventManager.instance.onSpellInvocation.AddListener(InvokeSpell);
    }

    void InvokeSpell(SpellType spellName)
    {
        StartCoroutine(SpellAnimation(spellName));
    }

    public IEnumerator SpellAnimation(SpellType spellName)
    {
        canPlay = false;
        Sc_EventManager.instance.onUpdateStats.Invoke();
        Sequence sequence = DOTween.Sequence();
        switch (spellName)
        {
            case SpellType.Ice:
                screen.color = Color.cyan;
                break;
            case SpellType.Fire:
                screen.color = Color.red;
                break;
            case SpellType.Shield:
                screen.color = Color.blue;
                break;
        }

        screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, 0);
        txt.color = new Color(0, 0, 0, 0);
        sequence.Append(screen.DOFade(0.5f, 0.3f));
        sequence.Append(txt.DOFade(1, 0.2f).SetDelay(0.3f));
        sequence.Play().SetAutoKill(false);
        txt.text = spellName.ToString();
        yield return new WaitForSeconds(sequence.Duration() * 2);
        sequence.PlayBackwards();
        yield return new WaitForSeconds(sequence.Duration());
        canPlay = true;
        Sc_EventManager.instance.onUpdateStats.Invoke();
    }

    public void GameOver(bool win)
    {
        screen.DOFade(0.5f, 0.8f);
        txt.DOFade(1, 0.3f).SetDelay(0.3f);

        if (win)
            txt.text = "Victory !!";
        else
            txt.text = "Game Over";
    }

    public void ChangeAction(int amount)
    {
        remainingActions += amount;
        if (remainingActions <= 0)
        {
            remainingActions = maxActions;
            StartCoroutine(LaunchTurn());
        }

        displayActions.text = remainingActions + "";
    }

    public IEnumerator LaunchTurn()
    {
        canPlay = false;
        Sc_EventManager.instance.onUpdateStats.Invoke();
        Vector3 baseScale = fightText.transform.localScale;
        float delay = 2f;
        fightText.DOFade(1, 0.3f);
        fightText.transform.DOScale(baseScale * 1.5f, delay);
        yield return new WaitForSeconds(delay);

        float wait = 3f;        
        mainPlayer.StartAttack(currentEnemy);
        yield return new WaitForSeconds(wait);
        if (!currentEnemy.isDead)
        {
            currentEnemy.StartAttack(mainPlayer);
            yield return new WaitForSeconds(wait);
        }

        fightText.DOFade(0, delay - 0.3f);
        fightText.transform.DOScale(baseScale, 0).SetDelay(delay - 0.3f);
        yield return new WaitForSeconds(delay - 0.3f);
        canPlay = true;
        Sc_EventManager.instance.onUpdateStats.Invoke();
    }
}
