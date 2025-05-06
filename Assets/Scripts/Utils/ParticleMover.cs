using EaseLibrary;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleMover : MonoBehaviour
{
    [SerializeField] private ParticleSystem myParticleSystem;
    [SerializeField] private float duration = 0;
    [SerializeField] private Vector3 startLoc;
    [SerializeField] private Vector3 endLoc;

    private void Awake()
    {
        
        if (myParticleSystem == null)
        {
            Debug.LogError("Particle system is not assigned!", this);
            return;
        }

        // Set the duration from the particle system
        var mainModule = myParticleSystem.main;
        duration = mainModule.duration;

        // Start animation if needed
        transform.position = startLoc;
        StartCoroutine(AnimateNewPosition());
    }

    [ContextMenu("Turn On")]
    public void Initiate(SlotHelper slotHelper)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (myParticleSystem == null)
        {
            Debug.LogError("Particle system is not assigned!", this);
            return;
        }

        if (slotHelper == null)
        { 
            SetTransforms(new Vector3(0, 2, -2.413f), new Vector3(0, 2, .365f));
        }
        else
        {
            (Vector3 loc1, Vector3 loc2) = slotHelper.GetSlotLocs();
            SetTransforms(loc1, loc2);

        }

        StartCoroutine(AnimateNewPosition());
    }

    public void SetTransforms(Vector3 startLocTransform, Vector3 endLocTransform)
    {
        startLoc = startLocTransform + transform.parent.position;
        endLoc = endLocTransform + transform.parent.position;
    }

    private IEnumerator AnimateNewPosition()
    {
        if (myParticleSystem == null)
        {
            yield break;
        }

        // Reset particle position and play
        myParticleSystem.Play();
        

        float timer = 0;
        Vector3 starting = startLoc;
        Vector3 ending = endLoc;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // Interpolate position with easing
            Vector3 newLoc = EaseFunctions.Interpolate(starting, ending, t, EaseFunctions.Ease.InCubic);
            transform.position = newLoc;

            yield return null;
        }

        // Ensure the final position is set
        transform.position = endLoc;

        // Optionally disable the object after the animation ends
        //gameObject.SetActive(false);
    }
}