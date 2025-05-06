[System.Serializable]
public class CardModifierData
{
    public string   modifierName;
    public string   description;
    public float    value;
    public bool     isActive;

    public CardModifierData(string modifierName, string description, float value, bool isActive)
    {
        this.modifierName = modifierName;
        this.description = description;
        this.value = value;
        this.isActive = isActive;
    }
}
