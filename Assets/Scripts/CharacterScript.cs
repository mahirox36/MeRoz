using UnityEngine;
using UnityEngine.UI;
public class CharacterScript : MonoBehaviour
{
    public CharacterData characterData;
    private Image image;

    private void Awake(){
        image = GetComponent<Image>();
    }
    public void ChangeSprite(string emotion){

        image.sprite = characterData.ToDialogueCharacter().sprites[emotion];
    }
    // Some Animations Function like in Ren'py
    // TODO: add entering animation: slide from left / right, fade in, from bottom, etc.
}