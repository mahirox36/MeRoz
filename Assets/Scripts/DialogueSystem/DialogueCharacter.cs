using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string characterName;
    public Color textColor = Color.white;
    public AudioClip voiceSound;
    public Dictionary<string, Sprite> sprites;
}