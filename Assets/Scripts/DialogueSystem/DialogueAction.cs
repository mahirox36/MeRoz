[System.Serializable]
public class DialogueAction
{
    public ActionType actionType;
    public string parameter;
}

public enum ActionType
{
    ChangeBackground,
    ChangeSprite,
    PlaySound
}
