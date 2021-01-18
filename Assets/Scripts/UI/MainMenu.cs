using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Text[] buttons = GetComponentsInChildren<Text>();
        for (int i = 0; i < buttons.Length; i++)
        {
            string newName = "Level 0" + (i + 1);
            buttons[i].text = newName;
            buttons[i].transform.root.name = newName;
        }
    }

    public void LaunchLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
