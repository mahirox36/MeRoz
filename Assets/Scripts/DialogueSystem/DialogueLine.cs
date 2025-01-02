using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public CharacterData speaker;
    public string text;
    public float speedTyping = -1f;
    public List<DialogueAction> actions = new();

    public DialogueCharacter GetSpeaker()
    {
        return speaker.ToDialogueCharacter();
    }
}