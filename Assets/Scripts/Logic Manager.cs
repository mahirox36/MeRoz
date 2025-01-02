using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class LogicManager : MonoBehaviour
{
    private Input inputs;
    private AudioSource audioSource;
    public GameObject panel;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI text;
    public List<DialogueSequence> sequences;
    public SpriteRenderer backgroundRenderer; // For changing backgrounds
    private DialogueSequence currentSequence;
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

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        panel.SetActive(true); 
        Name.text = sequences[0].lines[0].speaker.characterName;
        Name.color = sequences[0].lines[0].speaker.textColor;
        currentSequence = sequences[0];
        typingCoroutine = StartCoroutine(TypeText(currentSequence.lines[index].text, currentSequence.lines[index].speedTyping,
                currentSequence.lines[index].speaker.voiceSound));
    }

    public void ExecuteDialogueLine(DialogueLine line)
    {
        foreach (var action in line.actions)
        {
            switch (action.actionType)
            {
                case ActionType.ChangeBackground:
                    // ChangeBackground(action.parameter);
                    break;
                case ActionType.ChangeSprite:
                    // ChangeSprite(action.parameter);
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
            Name.text = currentSequence.lines[index].speaker.characterName;
            Name.color = currentSequence.lines[index].speaker.textColor;
            
            typingCoroutine = StartCoroutine(TypeText(currentSequence.lines[index].text, currentSequence.lines[index].speedTyping,
                currentSequence.lines[index].speaker.voiceSound));
        }
    }
}
