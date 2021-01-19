using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;

[Serializable]
class PathWay
{
    public string pathName;
    public Transform[] wayPoints;
}

public class Sc_Map : MonoBehaviour
{
    [SerializeField] GameObject helpPanel;
    Vector3 panelPosition;
    [SerializeField] GameObject playerIcon;
    [SerializeField] PathWay[] paths;
    bool done;
    Dictionary<string, PathWay> myPaths = new Dictionary<string, PathWay>();

    private void Start()
    {
        panelPosition = helpPanel.transform.position;
        Sc_EventManager.instance.onWin.AddListener(EndGame);
        foreach (var item in paths)
        {
            myPaths.Add(item.pathName, item);
        }
    }

    public void TranslatePanel(bool inCenter)
    {
        if (inCenter)
            helpPanel.transform.DOLocalMoveX(0, 0.5f);
        else
            helpPanel.transform.DOMove(panelPosition, 0.5f);
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
        float delay = 3.5f;
        yield return new WaitForSeconds(delay);
        if (victory)
        {
            RectTransform myTransform = GetComponent<RectTransform>();
            myTransform.DOLocalMoveY(0, 2f);
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
            PathWay thisPath = myPaths[sceneName];
            Vector3[] path = new Vector3[thisPath.wayPoints.Length];
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = thisPath.wayPoints[i].position;
            }

            sequence.Append(playerIcon.transform.DOPath(path, 0.5f * path.Length));
            sequence.Play();
            done = true;
            StartCoroutine(Load(sceneName, sequence.Duration() + 1));
            foreach (var item in GetComponentsInChildren<Button>())
            {
                item.interactable = false;
            }
        }
    }

    IEnumerator Load(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
