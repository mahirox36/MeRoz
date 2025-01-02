using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Sequence", menuName = "Dialogue/Sequence")]
public class DialogueSequence : ScriptableObject
{
    public string sequenceName;
    public List<DialogueLine> lines = new List<DialogueLine>();
}