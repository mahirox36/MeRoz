using UnityEngine;
using UnityEngine.UI;
public class CharacterScript : MonoBehaviour
{
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void ChangeSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
    // Some Animations Function like in Ren'py
    // TODO: add entering animation: slide from left / right, fade in, from bottom, etc.
}
