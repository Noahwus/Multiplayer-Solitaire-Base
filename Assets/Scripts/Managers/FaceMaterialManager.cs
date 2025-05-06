using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaceMaterialManager : Singleton<FaceMaterialManager>
{
    public Material[] FaceMats;
    public List<string> stdCardIndexing = new List<string>();
    public static string[] suits = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };
    public static string[] values = new string[] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

    public Material[] PowerUpFaceMats;
    public Material[] BackFaceMats;
    public Material[] BackFaceShape;

    public void Awake()
    {
        NewInstance(this);
    }

    public void Start()
    {
        GenerateCardNames();
    }

    public List<string> GenerateCardNames()
    {
        stdCardIndexing = new List<string>();
        List<CardTemplate> templates = CardFactory.CreateStandardCardTemplates();
        foreach (CardTemplate template in templates) 
        {
            stdCardIndexing.Add(template.name);
        }

        return stdCardIndexing;
    }
    
    public Material GetFaceMat(CardInstance card)
    { 
        if(stdCardIndexing.Count < 1){ GenerateCardNames(); }

        string baseName = card.GetDisplayName(); // in case card name includes suffix like "_1002"
        int index = stdCardIndexing.IndexOf(baseName);

        if (index >= 0 && index < FaceMats.Length)
        {
            return FaceMats[index];
        }

        Debug.LogWarning($"GetFaceMat: Couldn't find matching material for card: {card.GetDisplayName()}, using default.");
        return FaceMats[0];
    }

    public Material GetFaceMat(int i)
    {
        return FaceMats[i];

    }

    public Material GetFaceMat(string name)
    {
        if (name.Contains("Wild"))
        {
            return PowerUpFaceMats[0];
        }

        if (name.StartsWith("Card"))
        {
            Debug.Log(name + " was not present in getFaceMat()");
        }
        else
        {
            string[] nameParts = name.Split('_');

            string newName = nameParts[0];
            if (nameParts.Length > 3)
            {

                for (int i = 1; i < 3; i++)
                {
                    newName += "_" + nameParts[i];
                }
            }
            else
            {
                newName = name;
            }



            GenerateCardNames();
            int Index = -1;
            Index = stdCardIndexing.IndexOf(newName);
            if (Index == -1) { Index = stdCardIndexing.IndexOf(GetCardBaseName(newName)); }
            //Debug.Log(Index + " deck.IndexOf(name);");

            if (Index != -1) return FaceMats[Index];
        }

        Debug.Log("Returing Default Face Material, was given: " + name);
        return FaceMats[0];
    }


    public string GetCardBaseName(string cardName)
    {
        // Find the index of the last underscore in the card name
        int underscoreIndex = cardName.LastIndexOf('_');

        // If there's no underscore, return the original card name
        if (underscoreIndex == -1)
        {
            return cardName;
        }

        // Otherwise, return the substring before the underscore
        return cardName.Substring(0, underscoreIndex);
    }

    public (Material, Material) GetBackFaceMats(int playerIndex)
    {
        return (BackFaceMats[playerIndex],  BackFaceShape[playerIndex]);
    }
    
    public Material GetBackFaceMat (int playerIndex) => BackFaceMats[playerIndex];
    public Material GetBackFacePattern(int playerIndex) => BackFaceShape[playerIndex];
    

}
