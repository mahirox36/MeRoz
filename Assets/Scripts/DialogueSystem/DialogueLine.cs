using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public CharacterData speaker;
    public string text;
    public float speedTyping = 0.02f;

    public DialogueCharacter GetSpeaker()
    {
        return speaker.ToDialogueCharacter();
    }
}