﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Sc_Dialog : MonoBehaviour
{
    Sc_GameManager gameManager;
    Sc_Map map;
    [SerializeField] GameObject bubble;
    [SerializeField] Dialog dialogData;
    public bool playerSpeak = true;
    string[] linesToPlay;

    int count;
    Text mainText;
    CanvasGroup group;
    bool finished;
    bool defeat;

    private void Awake()
    {
        map = FindObjectOfType<Sc_Map>();
        gameManager = FindObjectOfType<Sc_GameManager>();
        gameManager.canPlay = false;
        mainText = GetComponentInChildren<Text>();
        group = GetComponent<CanvasGroup>();
        group.DOFade(0, 0);
        
        if (dialogData.startLines.Length > 0)
        {
            group.DOFade(1, 0.3f).SetDelay(0.3f);
            StartCoroutine(NewDialog(dialogData.startLines));
        }
    }

    private void Start()
    {
        Sc_EventManager.instance.onWin.AddListener(RestartDialog);
    }

    void RestartDialog(bool defeat)
    {
        this.defeat = defeat;
        playerSpeak = true;
        finished = false;
        linesToPlay = defeat ? dialogData.victoryLines : dialogData.defeatLines;
        group.DOFade(1, 0.3f).SetDelay(0.3f);
        StartCoroutine(NewDialog(linesToPlay));
    }

    IEnumerator NewDialog(string[] lines)
    {
        if (count < lines.Length)
        {
            Image bubbleImage = bubble.GetComponent<Image>();
            bubbleImage.DOComplete();
            bubbleImage.DOFade(0, 0.3f);
            bubbleImage.DOFade(1, 0.3f).SetDelay(0.3f);

            mainText.DOComplete();
            mainText.DOFade(0, 0.3f);
            mainText.DOFade(1, 0.3f).SetDelay(0.3f);
            mainText.DOText(lines[count], 0f).SetDelay(0.3f);
            count++;

            yield return new WaitForSeconds(0.15f);
            bubble.transform.rotation = playerSpeak ? Quaternion.Euler(0, -180, 0) : Quaternion.identity;
            playerSpeak = !playerSpeak;
        }
        else
        {
            group.DOFade(0, 0.3f);
            finished = true;
            gameManager.canPlay = true;
            count = 0;

            if (linesToPlay != null)
                map.EndGame(defeat);
            else
                Sc_EventManager.instance.onGameStart.Invoke();
        }
    }

    private void Update()
    {
        if (!finished)
        {
            gameManager.canPlay = false;
            if (Input.anyKeyDown)
            {
                if (linesToPlay != null && linesToPlay.Length > 0)
                    StartCoroutine(NewDialog(linesToPlay));
                else if (dialogData.startLines.Length > 0)
                    StartCoroutine(NewDialog(dialogData.startLines));
            }
        }
    }
}
