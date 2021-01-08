using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Sc_Score : MonoBehaviour
{
    Text myText => GetComponent<Text>();

    public static Sc_Score instance;
    [SerializeField] int currentScore;

    private void Awake()
    {
        instance = this;
        myText.text = currentScore.ToString();
    }

    public void ModifyScore(int amount)
    {
        currentScore += amount;
        myText.text = currentScore.ToString();
    }
}
