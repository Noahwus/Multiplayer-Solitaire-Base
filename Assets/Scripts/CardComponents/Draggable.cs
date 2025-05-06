using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Draggable : MonoBehaviour
{
    private Collider col;
    public bool isDragging = false;

    public Vector3 offset;
    public Vector3 targetLoc;
    private Vector3 originalPosition;
    
    public float dragSpeed = .05f;
    public float lerpSpeed = .1f;

    private Card card;

    private void Start()
    {
        col = GetComponent<Collider>();
        card = GetComponent<Card>();
    }

    private void Update()
    {
        if (isDragging && col.enabled == false) { col.enabled = false; }
        else if (col.enabled == false)          { col.enabled = true; }
    }

    public void DragMe()
    {
        isDragging = true;
    }

    public void DropMe()
    {
        isDragging = false;
    }

}
