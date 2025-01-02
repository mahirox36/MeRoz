using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class LogicManager : MonoBehaviour
{
    private Input inputs;
    private AudioSource audioSource;
    public GameObject panel;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI text;
    public List<DialogueSequence> sequences;
    private DialogueSequence currentSequence;
    private int index = 0;
    private int sequenceIndex = 0;

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
        text.text = sequences[0].lines[0].text;
    }
    
    void Next(InputAction.CallbackContext context) {
        if (sequenceIndex < sequences.Count) {
            currentSequence = sequences[sequenceIndex];
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
            text.text = currentSequence.lines[index].text;
            if (currentSequence.lines[index].speaker.voiceSound != null) {
                audioSource.clip = currentSequence.lines[index].speaker.voiceSound;
                audioSource.Play();
            }
        }
    }
}
