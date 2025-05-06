using System;
using UnityEngine;

public class Utils
{
    public static Card IsMouseOverCard()
    {
        GameObject obj = IsMouseOverObject();
        if(obj == null)
            { return null; }
        
        Card card = obj.GetComponent<Card>();
        
        if(card == null)
            { return null; }
        
        return card;
    }

    public static GameObject IsMouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray, out hit, 100))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    public static string GetTag(GameObject gm)
    {
        if (gm == null) { return null; }

        string tag = gm.tag;

        if (string.IsNullOrEmpty(tag)) { return null; }

        return tag;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }


    private static System.Random random = new System.Random();

    public static int ParseStringToInt(string str)
    {
        int result = -1;
        int.TryParse(str, out result);
        return result;
    }

    public static int Rand(params int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
        {
            throw new ArgumentException("At least one number must be provided.");
        }

        int randomIndex = random.Next(0, numbers.Length); // Generate a random index
        return numbers[randomIndex]; // Return the number at the random index
    }


    public static string ColorText(string text, Color color)
    {
        // Convert the color to a hex string
        string hexColor = UnityEngine.ColorUtility.ToHtmlStringRGB(color);

        // Wrap the text with the <color> tag
        return $"<color=#{hexColor}>{text}</color>";
    }

    public static string TF(bool trufalse, string text = "")
    {
        if (string.IsNullOrEmpty(text)) { text = trufalse.ToString(); }
        if (trufalse) { return ColorText(text, Color.green); }
                        return ColorText(text, Color.red);

    }

    public static string BoldText(string text)
    {
        // Wrap the text with the <b> tags
        return $"<b>{text}</b>";
    }

    

}
