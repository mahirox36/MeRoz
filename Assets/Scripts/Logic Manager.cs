using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LogicManager : MonoBehaviour
{

    [System.Serializable]
    public class JsonEntry
    {
        public string type;
        public string action;
        public string character;
        public string background;
        public float posX;
        public string text;
        public float speed;
    }

    public GameObject Character;
    public List<DialogueSequence> sequences = new();
    public List<JsonEntry> JsonData;
    public string dataName;

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

    void LoadJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"Dialogues/{dataName}.json");
        Debug.Log(path);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // Deserialize into a list of JsonEntry objects
            JsonData = JsonConvert.DeserializeObject<List<JsonEntry>>(json);
            Debug.Log(JsonConvert.SerializeObject(JsonData, Formatting.Indented)); // Pretty print for debugging


        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }

    private void LoadBackgroundByName(string backgroundName)
    {
        Sprite backgroundSprite = Resources.Load<Sprite>($"Backgrounds/{backgroundName}");
        if (backgroundSprite != null)
        {
            background.sprite = backgroundSprite;
        }
        else
        {
            Debug.LogError($"Background with name {backgroundName} not found.");
        }
    }

    private void LoadSoundByName(string soundName)
    {
        AudioClip soundClip = Resources.Load<AudioClip>($"Sounds/{soundName}");
        if (soundClip != null)
        {
            audioSource.clip = soundClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError($"Sound with name {soundName} not found.");
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
        LoadJson();
        // currentSequence = sequences[0];
        // SetSpeaker();
        // if (currentSequence.lines[index].text != "") {
        //     typingCoroutine = StartCoroutine(TypeText(currentSequence.lines[index].text, currentSequence.lines[index].speedTyping,
        //     currentSequence.lines[index].speaker.voiceSound));
        //     }
        // else{
        //     Next(new InputAction.CallbackContext());
        // }
        // CreateCharacter(sequences[0].lines[0].speaker);

    }

    private CharacterData LoadCharacterDataByName(string characterName)
    {
        CharacterData[] allCharacters = Resources.LoadAll<CharacterData>("Characters");
        foreach (CharacterData character in allCharacters)
        {
            if (character.characterName == characterName)
            {
                return character;
            }
        }
        Debug.LogError($"CharacterData with name {characterName} not found.");
        return null;
    }

    private void CreateCharacter(CharacterData characterData, int positionX, string emotion = "idle")
    {
        GameObject newCharacter = Instantiate(Character, charactersParent.transform);
        newCharacter.transform.SetSiblingIndex(1);
        newCharacter.name = characterData.name;
        newCharacter.GetComponent<CharacterScript>().characterData = characterData;
        newCharacter.GetComponent<CharacterScript>().ChangeSprite(emotion);
        newCharacter.GetComponent<Image>().SetNativeSize();
        newCharacter.transform.localPosition = new Vector3(positionX, newCharacter.transform.position.y, newCharacter.transform.position.z);
        characters.Add(characterData.name, newCharacter);
    }

    private void DeleteCharacter(CharacterData characterData)
    {
        GameObject character = characters[characterData.name];
        characters.Remove(characterData.name);
        Destroy(character);
    }



    // public void ExecuteDialogueLine(DialogueLine line)
    // {
    //     foreach (var action in line.actions)
    //     {
    //         switch (action.actionType)
    //         {
    //             case ActionType.ChangeBackground:
    //                 background.sprite = action.image;
    //                 break;
    //             case ActionType.ChangeEmotion:
    //                 if (!characters.ContainsKey(action.character.name)) {
    //                     Debug.LogError($"Character {action.character.name} not found");
    //                 }
    //                 characters[action.character.name].GetComponent<CharacterScript>().ChangeSprite(action.parameter);
    //                 break;
    //             case ActionType.CreateCharacter:
    //                 CreateCharacter(action.character, action.positionX, action.parameter);
    //                 break;
    //             case ActionType.DeleteCharacter:
    //                 DeleteCharacter(action.character);
    //                 break;
    //             case ActionType.PlaySound:
    //                 // PlaySound(action.parameter);
    //                 break;
    //         }
    //     }
    // }

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
