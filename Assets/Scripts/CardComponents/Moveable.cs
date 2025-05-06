using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EaseLibrary;

public class Moveable : MonoBehaviour
{
    Coroutine isMoving;
    public float defaultLerpTime = .05f;
    public EaseFunctions.Ease moveEase;

    public void EnumLerpToPosition(Vector3 tar, float AltLerpTime = -1)
    {
        if (tar == null) return;
        if (isMoving != null) { StopCoroutine(isMoving); }
        isMoving = StartCoroutine(LerpToPosition(tar, AltLerpTime));
    }

    public IEnumerator LerpToPosition(Vector3 tar, float AltLerpTime)
    {
        float lerpTime = (AltLerpTime > 0) ? AltLerpTime : defaultLerpTime;

        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < lerpTime)
        {
            float t = elapsed / lerpTime;
            transform.position = EaseFunctions.Interpolate(startPosition, tar, t, moveEase);
          
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = tar; // Snap exactly to targetLocation at end
        isMoving = null;
    }

    public bool IsCardAtTargetPosition(Vector3 targetPosition, float threshold = 0.01f)
    {
        return Vector3.Distance(transform.position, targetPosition) <= threshold;
    }
}
