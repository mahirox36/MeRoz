using UnityEngine;

[System.Serializable]
public class CreateCharacter
{
    public CharacterData character;
    public int positionX;
}
[System.Serializable]
public class DeleteCharacter
{
    public CharacterData character;
}
[System.Serializable]
public class ChangeEmotion
{
    public CharacterData character;
    public Sprite emotion;
}
[System.Serializable]
public class ChangeBackground
{
    public Sprite background;
}
[System.Serializable]
public class PlaySound
{
    public AudioClip sound;
}

public enum AnimationType
{
    None,
    FadeIn,
    SlideIn,
    FadeOut,
    SlideOut
}
