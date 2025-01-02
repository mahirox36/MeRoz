using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class LogicManager : MonoBehaviour
{
    private Input inputs;
    private AudioSource audioSource;
    public GameObject panel;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI text;
    public List<DialogueLine> Lines;
    private int index = 0;

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
        Name.text = Lines[index].speaker.characterName;
        Name.color = Lines[index].speaker.textColor;
        text.text = Lines[index].text;
        
    }
    
    void Next(InputAction.CallbackContext context) {
        if (index < Lines.Count - 1) {
            index++;
            Name.text = Lines[index].speaker.characterName;
            Name.color = Lines[index].speaker.textColor;
            text.text = Lines[index].text;
            if (Lines[index].speaker.voiceSound != null) {
                audioSource.clip = Lines[index].speaker.voiceSound;
                audioSource.Play();
            }
        }
    }   
}
