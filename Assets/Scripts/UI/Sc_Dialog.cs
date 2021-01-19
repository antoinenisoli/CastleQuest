using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Sc_Dialog : MonoBehaviour
{
    Sc_GameManager gameManager;
    [SerializeField] GameObject bubble;
    [SerializeField] Transform[] positions;
    [TextArea] [SerializeField] string[] startLines;
    [TextArea] [SerializeField] string[] victoryLines;
    [TextArea] [SerializeField] string[] defeatLines;
    string[] linesToPlay;
    int count;
    Text mainText;
    CanvasGroup group;
    bool finished;
    bool playerSpeak = true;

    private void Awake()
    {
        gameManager = FindObjectOfType<Sc_GameManager>();
        mainText = GetComponentInChildren<Text>();
        group = GetComponent<CanvasGroup>();
        group.DOFade(0, 0);
        
        if (startLines.Length > 0)
        {
            group.DOFade(1, 0.3f).SetDelay(0.3f);
            StartCoroutine(NewDialog(startLines));
        }
    }

    private void Start()
    {
        Sc_EventManager.instance.onWin.AddListener(RestartDialog);
    }

    void RestartDialog(bool defeat)
    {
        playerSpeak = defeat;
        finished = false;
        linesToPlay = defeat ? victoryLines : defeatLines;
        StartCoroutine(NewDialog(linesToPlay));
    }

    IEnumerator NewDialog(string[] lines)
    {
        if (count < startLines.Length)
        {
            Image bubbleImage = bubble.GetComponent<Image>();
            bubbleImage.DOComplete();
            bubbleImage.DOFade(0, 0.3f);
            bubbleImage.DOFade(1, 0.3f).SetDelay(0.3f);

            mainText.DOComplete();
            mainText.DOFade(0, 0.3f);
            mainText.DOFade(1, 0.3f).SetDelay(0.3f);
            mainText.DOText(lines[count], 0.3f).SetDelay(0.15f);
            count++;

            yield return new WaitForSeconds(0.15f);
            bubble.transform.rotation = playerSpeak ? Quaternion.Euler(0, -180, 0) : Quaternion.identity;
            bubble.transform.position = playerSpeak ? positions[0].position : positions[1].position;
            mainText.transform.position = bubble.transform.position;
            playerSpeak = !playerSpeak;
        }
        else
        {
            group.DOFade(0, 0.3f);
            finished = true;
            gameManager.canPlay = true;
            count = 0;
        }
    }

    private void Update()
    {
        if (!finished)
        {
            gameManager.canPlay = false;
            if (Input.anyKeyDown)
            {
                if (linesToPlay.Length > 0)
                    StartCoroutine(NewDialog(linesToPlay));
                else if (startLines.Length > 0)
                    StartCoroutine(NewDialog(startLines));
            }
        }
    }
}
