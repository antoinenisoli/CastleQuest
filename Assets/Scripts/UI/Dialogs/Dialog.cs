using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog_LEVEL", menuName = "Dialogs/New Dialog")]
public class Dialog : ScriptableObject
{
    [TextArea] public string[] startLines;
    [TextArea] public string[] victoryLines;
    [TextArea] public string[] defeatLines;
}
