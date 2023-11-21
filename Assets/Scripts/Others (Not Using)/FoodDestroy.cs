using UnityEngine;

public class FoodDestroy : MonoBehaviour
{
    private float pickupRadius;

    public void SetPickupRadius(float radius)
    {
        pickupRadius = radius;
    }

    void Update()
    {
        // Check for nearby agents (customers)
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
        foreach (Collider collider in colliders)
        {
            // Check if the collider belongs to an agent (customize the tag as needed)
            if (collider.CompareTag("Agent"))
            {
                // Food has been picked up, destroy it
                Debug.Log("Food picked up by agent.");
                Destroy(gameObject);
                break; // Exit the loop once a pickup is detected
            }
        }
    }
}