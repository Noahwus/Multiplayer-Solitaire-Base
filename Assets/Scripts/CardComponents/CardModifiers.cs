using UnityEngine;

/// <summary>
/// CardModifer will serve as an "add on" to cards later, when we upgrade cards with Power Ups. This is largely placeholder
/// </summary>

[System.Serializable]
public class CardModifier  
{
    public string modifierName  { get; private set; }
    public string description   { get; private set; }
    public float value          { get; private set; }   // Optional effect magnitude
    public bool isActive        { get; private set; }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public CardModifier(string modName, string description = "", float value = -1f)
    {
        modifierName = modName;
        this.description = description;
        this.value = value;
        isActive = true;
    }

} 
