using UnityEngine;

//[ExecuteAlways] // Allows scaling in both Play Mode and Edit Mode
public class MatchBoxCollider : MonoBehaviour
{
    public BoxCollider parentCollider; // Reference to the parent's BoxCollider
    public Vector3 originalLoc;

   //public float scaley = 0.27f;
    public Vector3 Scaler = new Vector3(1,1, .27f);

    private void Start()
    {
        originalLoc = transform.localPosition;
    }
    void Update()
    {
        return;
        if (parentCollider == null)
        {
            // Automatically find the parent BoxCollider if not assigned
            parentCollider = GetComponentInParent<BoxCollider>();
            if (parentCollider == null)
            {
                Debug.LogError("No BoxCollider found in parent!");
                return;
            }
        }

        // Match the size and center of the parent BoxCollider
        Vector3 colliderSize = parentCollider.size;
        Vector3 colliderScale = parentCollider.transform.localScale;

        // Adjust child local scale based on the parent's collider size and scale
        transform.localScale = new Vector3(
            Scaler.x,
            Scaler.y,
            Scaler.z + (colliderSize.z * colliderScale.z)
        );

        // Match the position of the child to the BoxCollider center (optional)
        transform.localPosition = new Vector3(parentCollider.center.x, originalLoc.y, parentCollider.center.z);
            //parentCollider.center;
    }
}
