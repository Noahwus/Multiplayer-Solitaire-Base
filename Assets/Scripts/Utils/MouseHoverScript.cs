using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverScript : MonoBehaviour
{
    public GameObject childObject;
    public BoxCollider col;

    private void Start()
    {
        col = GetComponent<BoxCollider>();
        childObject = transform.GetChild(0).gameObject;
        childObject.gameObject.SetActive(false);
    }
    private void Update()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit; //HOPE THIS DIDNT FUCK ANYTHING UP

        // Check if the ray hits the bounds of the object
        if (col.bounds.IntersectRay(ray))
        {
            // The mouse is hovering over the object, so enable the child object
            childObject.SetActive(true);
            //SoundManager.Instance.PlaySound(4);
        }
        else
        {
            // The mouse is not hovering, so disable the child object
            childObject.SetActive(false);
        }
    }
}
