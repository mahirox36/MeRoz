using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LogicManager : MonoBehaviour
{
    public GameObject Character;
    public List<DialogueSequence> sequences;

    private Input inputs;
    private Image background;
    private GameObject panel;
    private GameObject charactersParent;
    private Dictionary<string, GameObject> characters = new();
    private DialogueSequence currentSequence;
    private TextMeshProUGUI Name;
    private TextMeshProUGUI text;
    private AudioSource audioSource;
    private int index = 0;
    private int sequenceIndex = 0;
    private Coroutine typingCoroutine;

    private void Awake() {
        inputs = new();
    }
    
    private void OnEnable() {
        inputs.Enable();
        inputs.Player.Next.performed += Next;
    }
    private void OnDisable() {
        inputs.Disable();
    }
    public void SetSpeaker() {
        if (currentSequence.lines[index].speaker != null) {
            Name.text = currentSequence.lines[index].speaker.characterName;
            Name.color = currentSequence.lines[index].speaker.textColor;
        }else{
            Name.text = "";
            Name.color = Color.white;
        }
    }

    private void Start()
    {
        // Character is a prefab of a character with an Image component 1
        background = GameObject.Find("Background").GetComponent<Image>();
        panel = GameObject.Find("DialoguePanel");
        charactersParent = GameObject.Find("Canvas");
        Name = panel.GetComponentsInChildren<TextMeshProUGUI>()[0];
        text = panel.GetComponentsInChildren<TextMeshProUGUI>()[1];
        audioSource = GetComponent<AudioSource>();
        panel.SetActive(true);
        currentSequence = sequences[0];
        SetSpeaker();
        if (currentSequence.lines[index].text != "") {
            typingCoroutine = StartCoroutine(TypeText(currentSequence.lines[index].text, currentSequence.lines[index].speedTyping,
            currentSequence.lines[index].speaker.voiceSound));
            }
        else{
            ExecuteDialogueLine(currentSequence.lines[index]);
            Next(new InputAction.CallbackContext());
        }
        // CreateCharacter(sequences[0].lines[0].speaker);
    }

    private void CreateCharacter(CharacterData characterData, string emotion = "idle")
    {
        GameObject newCharacter = Instantiate(Character, charactersParent.transform);
        newCharacter.transform.SetSiblingIndex(1);
        newCharacter.name = characterData.name;
        newCharacter.GetComponent<CharacterScript>().characterData = characterData;
        newCharacter.GetComponent<CharacterScript>().ChangeSprite(emotion);
        characters.Add(characterData.name, newCharacter);
    }

    private void DeleteCharacter(CharacterData characterData)
    {
        GameObject character = characters[characterData.name];
        characters.Remove(characterData.name);
        Destroy(character);
    }



    public void ExecuteDialogueLine(DialogueLine line)
    {
        foreach (var action in line.actions)
        {
            switch (action.actionType)
            {
                case ActionType.ChangeBackground:
                    background.sprite = action.image;
                    break;
                case ActionType.ChangeEmotion:
                    if (!characters.ContainsKey(action.character.name)) {
                        Debug.LogError($"Character {action.character.name} not found");
                    }
                    characters[action.character.name].GetComponent<CharacterScript>().ChangeSprite(action.parameter);
                    break;
                case ActionType.CreateCharacter:
                    CreateCharacter(action.character, action.parameter);
                    break;
                case ActionType.DeleteCharacter:
                    DeleteCharacter(action.character);
                    break;
                case ActionType.PlaySound:
                    // PlaySound(action.parameter);
                    break;
            }
        }
    }

    private IEnumerator TypeText(string textToType, float typingSpeed, AudioClip audio) {
        text.text = "";
        float timeSinceLastSound = 0f;
        float minTimeBetweenSounds = typingSpeed + 0.01f;

        foreach (char letter in textToType) {
            text.text += letter;

            if (audio != null && 
                char.IsLetterOrDigit(letter) && 
                timeSinceLastSound >= minTimeBetweenSounds) {
                
                audioSource.PlayOneShot(audio, 0.5f); // 0.5f reduces volume to 50%
                timeSinceLastSound = 0f;
            }

            timeSinceLastSound += typingSpeed;
            if (letter == '.' || letter == '!' || letter == '?') {
                yield return new WaitForSeconds(typingSpeed * 30);
            } else if (letter == ',') {
                yield return new WaitForSeconds(typingSpeed * 10);
            }
            else {
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    void Next(InputAction.CallbackContext context) {
        if (sequenceIndex < sequences.Count) {
            currentSequence = sequences[sequenceIndex];
            if (typingCoroutine != null) {
                StopCoroutine(typingCoroutine);
                Debug.Log($"{typingCoroutine} stopped");
                typingCoroutine = null;
                text.text = currentSequence.lines[index].text;
                return;
            }
            if (index < currentSequence.lines.Count - 1) {
                index++;
            } else {
                index = 0;
                sequenceIndex++;
                if (sequenceIndex >= sequences.Count) {
                    panel.SetActive(false);
                    return;
                }
                currentSequence = sequences[sequenceIndex];
                
            }   
            ExecuteDialogueLine(currentSequence.lines[index]);
            if (currentSequence.lines[index].text == "") {
                Next(new InputAction.CallbackContext());
                return;
            }
            SetSpeaker();
            if (currentSequence.lines[index].speaker == null) {
                typingCoroutine = StartCoroutine(TypeText(currentSequence.lines[index].text, currentSequence.lines[index].speedTyping, null));
            }else{
            typingCoroutine = StartCoroutine(TypeText(currentSequence.lines[index].text, currentSequence.lines[index].speedTyping,
                currentSequence.lines[index].speaker.voiceSound));
                }
        }
    }
}
