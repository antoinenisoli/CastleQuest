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

    private void Awake()
    {
        mainPlayer = FindObjectOfType<Sc_Player>();
        currentEnemy = FindObjectOfType<Sc_Enemy>();
        remainingActions = maxActions;
        displayActions.text = remainingActions + "";
        fightText.DOFade(0, 0);

        endScreen.GetComponentInChildren<Image>().DOFade(0, 0);
        endScreen.GetComponentInChildren<Text>().DOFade(0, 0);
    }

    private void Start()
    {
        Sc_EventManager.instance.onWin.AddListener(GameOver);
    }

    public void GameOver(bool win)
    {
        Image screen = endScreen.GetComponentInChildren<Image>();
        Text txt = endScreen.GetComponentInChildren<Text>();
        screen.DOFade(0.5f, 0.8f);
        txt.DOFade(1, 0.3f).SetDelay(0.3f);

        if (win)
            txt.text = "Victory !!";
        else
            txt.text = "Game Over";
    }

    public void RemoveAction()
    {
        remainingActions--;
        if (remainingActions <= 0)
        {
            remainingActions = maxActions;
            StartCoroutine(LaunchTurn());
        }

        displayActions.text = remainingActions + "";
    }

    public IEnumerator LaunchTurn()
    {
        Vector3 baseScale = fightText.transform.localScale;
        float delay = 2f;
        fightText.DOFade(1, 0.3f);
        fightText.transform.DOScale(baseScale * 1.5f, delay);
        yield return new WaitForSeconds(delay);

        float wait = 3f;        
        mainPlayer.StartAttack(currentEnemy);        
        if (currentEnemy.GetLife.Value > 0)
        {
            yield return new WaitForSeconds(wait);
            currentEnemy.StartAttack(mainPlayer);
            yield return new WaitForSeconds(wait);
        }

        fightText.DOFade(0, delay - 0.3f);
        fightText.transform.DOScale(baseScale, 0).SetDelay(delay - 0.3f);
        yield return new WaitForSeconds(delay - 0.3f);
    }
}
