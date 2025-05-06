using System.Collections.Generic;
using UnityEngine;

public static class DeckUtils
{
    
    /// Useful for single-player or local-only use.
    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
    }

    //Seeded
    public static void ShuffleWithSeed<T>(List<T> list, int seed)
    {
        System.Random rng = new System.Random(seed);
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public static List<T> CutDeck<T>(List<T> deck, int index)
    {
        if (deck == null || deck.Count == 0 || index < 0 || index >= deck.Count)
            return new List<T>(deck); // Return a copy as-is if invalid

        List<T> cutDeck = new List<T>();

        // Add bottom half first (from cut point to end)
        cutDeck.AddRange(deck.GetRange(index, deck.Count - index));

        // Then add top half (from start to cut point)
        cutDeck.AddRange(deck.GetRange(0, index));

        return cutDeck;
    }

    public static List<List<T>> SplitDeck<T>(List<T> deck, int parts)
    {
        List<List<T>> result = new List<List<T>>();
        if (deck == null || deck.Count == 0 || parts <= 0)
            return result;

        int size = deck.Count / parts;
        int remainder = deck.Count % parts;

        int startIndex = 0;
        for (int i = 0; i < parts; i++)
        {
            int chunkSize = size + (i < remainder ? 1 : 0); // Distribute remainders to first N parts
            result.Add(deck.GetRange(startIndex, chunkSize));
            startIndex += chunkSize;
        }

        return result;
    }
    public static List<T> CloneDeck<T>(List<T> original)
    {
        return new List<T>(original);
    }
}


