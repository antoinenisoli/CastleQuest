using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Sc_Map : MonoBehaviour
{
    [SerializeField] GameObject playerIcon;
    bool done;

    private void Start()
    {
        Sc_EventManager.instance.onWin.AddListener(EndGame);
    }

    void EndGame(bool b)
    {
        StartCoroutine(ShowMap(b));
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator ShowMap(bool victory)
    {
        float delay = 2;
        yield return new WaitForSeconds(delay);
        if (victory)
        {
            RectTransform myTransform = GetComponent<RectTransform>();
            myTransform.DOLocalMoveY(0, 1.5f);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void LaunchLevel(string sceneName)
    {
        if (!done)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(playerIcon.GetComponent<RectTransform>().DOMove(EventSystem.current.currentSelectedGameObject.transform.position, 2f));
            sequence.Play();
            done = true;
            StartCoroutine(Load(sceneName, sequence.Duration() + 1));
        }
    }

    IEnumerator Load(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
