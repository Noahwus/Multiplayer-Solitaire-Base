using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [Header("References")]
    public GameObject   cardVisuals;
    public GameObject   cardVisualScaler;
    public GameObject   cardFace;
    [Space]
    [Header("Rendering")]
    private Material    faceMat;
    private Material    backFaceMat;
    private Material    backFaceShape;
    [Space]
    public GameObject faceGameObject;
    public GameObject backFaceColorGameObject;
    public GameObject backFacePatternGameObject;

    public Renderer     faceRenderer;
    public Renderer     backFaceColorRenderer;
    public Renderer     backFacePatternRenderer;

    [Space]
    [Header("Behaviours")]
    public AnimationCurve rotationCurve;
    public float flipDuration = .6f;
    public bool isFlipping = false;
    public bool isFaceUp = false;

    [Space]
    public AnimationCurve shakeCurve;
    public float shakeRange = 30f;
    public float shakeDuration = 1f;
    public bool isShaking = false;

    [Space]
    public AnimationCurve pulseCurve;
    public float pulseDuration = .15f;
    public float pulseRange = 1.19f;
    public bool isPulsing = false;

    public void SetCardFace(Material cardMaterial, Renderer renderer = null)
    {
        if (renderer == null || cardMaterial == null) 
        {   
            if (renderer == null){ renderer = faceRenderer; }
            if (cardMaterial == null) { Debug.Log("Material is null."); return; }
        }
        renderer.material = cardMaterial;
    }

    public void Flip()
    {
        if (!isFaceUp)
        { cardVisuals.transform.eulerAngles = new Vector3(cardVisuals.transform.eulerAngles.x, cardVisuals.transform.eulerAngles.y, 0); }
        else
        { cardVisuals.transform.eulerAngles = new Vector3(cardVisuals.transform.eulerAngles.x, cardVisuals.transform.eulerAngles.y, 180); }

        isFaceUp = !isFaceUp;

        StartCoroutine(FlipCard());
    }

    public IEnumerator FlipCard(float flipdur = -1)
    {
        isFlipping = true;

        float startZ = cardVisuals.transform.eulerAngles.z;
        float targetZ = startZ + 180f; // Target Z rotation
        float elapsedTime = 0f;

        if ((flipdur < 0)) { flipdur = flipDuration; }

        while (elapsedTime < flipdur)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / flipDuration);
            float curveValue = rotationCurve.Evaluate(t);

            // Instead of using Lerp, calculate the new rotation based on curveValue
            float currentZ = startZ + (curveValue * 180f);
            cardVisuals.transform.eulerAngles = new Vector3(cardVisuals.transform.eulerAngles.x, cardVisuals.transform.eulerAngles.y, currentZ);

            yield return null;
        }

        cardVisuals.transform.eulerAngles = new Vector3(cardVisuals.transform.eulerAngles.x, cardVisuals.transform.eulerAngles.y, targetZ);
        isFlipping = false;
    }

    public void Shake()
    {
        if (isShaking)
        {
            return;
        }

        StopCoroutine(ShakeCoroutine());
        StartCoroutine(ShakeCoroutine());
    }

    public IEnumerator ShakeCoroutine()
    {
        //Reset values, incase this is called during a previous shake
        float z = cardVisuals.transform.eulerAngles.z;
        cardVisuals.transform.rotation = Quaternion.Euler(0, 0, z);

        float duration = shakeDuration;
        float timer = 0f;

        isShaking = true;
        while (timer < duration)
        {
            float curveValue = shakeCurve.Evaluate(timer / duration);
            float yRotation = curveValue * shakeRange;

            cardVisuals.transform.rotation = Quaternion.Euler(0, yRotation, cardVisuals.transform.eulerAngles.z);

            yield return null;
            timer += Time.deltaTime;
        }

        cardVisuals.transform.rotation = Quaternion.Euler(0, 0, cardVisuals.transform.eulerAngles.z);
        isShaking = false;
    }

    public void Pulse()
    {
        if (this.isPulsing)
        { 
            StopCoroutine(PulseCoroutine());
            ResetScale();
        }
        
        StartCoroutine(PulseCoroutine());
    }
    public IEnumerator PulseCoroutine()
    {
        isPulsing = true;

        Transform scaler = cardVisualScaler.transform;
        Vector3 originalScale = scaler.localScale;
        Vector3 targetScale = originalScale * pulseRange;

        float elapsedTime = 0f;

        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / pulseDuration);
            float curveValue = pulseCurve.Evaluate(t);
            scaler.localScale = Vector3.Lerp(originalScale, targetScale, curveValue);
            yield return null;
        }

        scaler.localScale = originalScale;
        isPulsing = false;
        Debug.Log("Pulse complete");
    }

    public void ResetScale()
    {
        cardVisualScaler.transform.localScale = new Vector3(1, 1, 1);
    }
    
}