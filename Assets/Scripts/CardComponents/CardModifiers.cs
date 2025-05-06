using UnityEngine;

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



    #region Valuable Data Yeet
    // Set Up Data // Serialize Data // Deserialize Data
    public CardModifier(string modName, string description = "", float value = -1f)
    {
        modifierName = modName;
        this.description = description;
        this.value = value;
        isActive = true;
    }

    public CardModifierData ToData()
    {
        return new CardModifierData(modifierName, description, value, isActive);
    }

    public static CardModifier FromData(CardModifierData data)
    {
        CardModifier modifier = new CardModifier(data.modifierName)
        {
            description = data.description,
            value = data.value
        };

        if (data.isActive)
            modifier.Activate();
        else
            modifier.Deactivate();

        return modifier;
    }
    #endregion
} 
