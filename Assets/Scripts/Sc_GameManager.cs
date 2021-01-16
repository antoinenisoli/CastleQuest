using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_GameManager : MonoBehaviour
{
    Sc_Player mainPlayer;
    Sc_Enemy currentEnemy;
    [SerializeField] int maxActions = 15;
    int remainingActions;
    [SerializeField] Text displayActions;
    public bool canPlay = true;

    private void Awake()
    {
        mainPlayer = FindObjectOfType<Sc_Player>();
        currentEnemy = FindObjectOfType<Sc_Enemy>();
        remainingActions = maxActions;
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
        canPlay = false;
        mainPlayer.Attack(currentEnemy);
        Sc_EventManager.instance.onUpdateStats.Invoke();
        yield return new WaitForSeconds(1);
        currentEnemy.Attack(mainPlayer);
        canPlay = true;
        Sc_EventManager.instance.onUpdateStats.Invoke();
    }
}
