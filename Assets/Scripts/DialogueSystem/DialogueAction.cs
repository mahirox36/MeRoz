using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueAction
{
    public ActionType actionType;
    public string parameter;
    public Sprite image;
    public AudioClip audio;
    public CharacterData character;
    public AnimationType characterAnimation;
}

public enum ActionType
{
    ChangeBackground,
    ChangeEmotion,
    CreateCharacter,
    DeleteCharacter,
    PlaySound
}

public enum AnimationType
{
    None,
    FadeIn,
    SlideIn,
    FadeOut,
    SlideOut
}
