using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Color textColor = Color.white;
    public AudioClip voiceSound;

    [System.Serializable]
    public class EmotionSpritePair
    {
        public string emotion;
        public Sprite sprite;
    }

    public List<EmotionSpritePair> emotionSprites = new List<EmotionSpritePair>();

    public DialogueCharacter ToDialogueCharacter()
    {
        var spriteDictionary = new Dictionary<string, Sprite>();
        foreach (var pair in emotionSprites)
        {
            if (!spriteDictionary.ContainsKey(pair.emotion))
            {
                spriteDictionary.Add(pair.emotion, pair.sprite);
            }
        }

        return new DialogueCharacter
        {
            characterName = characterName,
            textColor = textColor,
            voiceSound = voiceSound,
            sprites = spriteDictionary
        };
    }
}
