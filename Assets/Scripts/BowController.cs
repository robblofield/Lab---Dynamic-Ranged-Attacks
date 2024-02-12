using UnityEngine;

public class BowController : MonoBehaviour
{
    public GameObject arrowPrefab;
    public TrajectoryVisualizer trajectoryVisualizer;
    public Transform arrowSpawnPoint;

    private float currentPullback = 0f; // Current pullback distance

    public float minPullback = 0.5f; // Minimum pullback distance
    public float maxPullback = 3f; // Maximum pullback distance
    public float maxDrawbackTime = 2f; // Maximum drawback time
    public float maxProjectileSpeed = 20f; // Maximum projectile speed
    public float maxDamageMultiplier = 3f; // Maximum damage multiplier

    private bool isPulling = false;
    private float drawbackStartTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            StartPulling(); // Call StartPulling when shooting action is initiated
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseArrow();
        }

        if (isPulling)
        {
            PullBow();
            UpdatePull();
        }
    }

    public void StartPulling()
    {
        isPulling = true;
        drawbackStartTime = Time.time;
    }

    void UpdatePull()
    {
        // Calculate pullback distance continuously while pulling
        float drawbackTime = Time.time - drawbackStartTime;
        currentPullback = Mathf.Clamp(drawbackTime, 0f, maxDrawbackTime) / maxDrawbackTime * (maxPullback - minPullback) + minPullback;

        // Calculate arrow direction based on camera rotation
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 arrowDirection = Vector3.RotateTowards(cameraForward, cameraRight, currentPullback, 0f); // Rotate camera forward towards right by currentPullback amount

        Debug.Log("Arrow Direction: " + arrowDirection);

    }


    public void ReleaseArrow()
    {
        isPulling = false;

        // Calculate projectile speed and damage multiplier based on pullback
        float projectileSpeed = currentPullback / maxPullback * maxProjectileSpeed;
        float damageMultiplier = currentPullback / maxPullback * maxDamageMultiplier;

        // Spawn arrow and set its properties
        Vector3 arrowDirection = Camera.main.transform.forward * currentPullback;
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(arrowDirection));
        Rigidbody arrowRigidbody = arrow.GetComponent<Rigidbody>();
        
        arrowRigidbody.velocity = arrowDirection * projectileSpeed;

        ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
        arrowScript.SetDamageMultiplier(damageMultiplier);

        Destroy(arrow, 5f); // Destroy arrow after 5 seconds

       
    }

    private void PullBow()
    {
        // Implement pulling back the bow
        // This could involve animating the bowstring or adjusting its position
        currentPullback += Time.deltaTime;
        currentPullback = Mathf.Clamp(currentPullback, 0f, maxPullback);
    }
}
