using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private float totalDamage = 0f; // Total damage to be applied

    public void SetDamage(float damage)
    {
        totalDamage = damage;
    }

    private void FixedUpdate()
    {
        // Cast a ray forward from the arrow's tip
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            // Check if the ray hit a valid target
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Arrow hit target! Damage dealt: " + totalDamage);
                // Apply damage to the hit object here
            }
            else if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.Log("Arrow hit obstacle!");
                // Handle logic for arrow hitting an obstacle (e.g., stop arrow, ricochet)
                // You can cast additional rays for other directions here
                // For example, cast rays to the left, right, up, and down from the arrow's midpoint
            }
        }
    }
}
