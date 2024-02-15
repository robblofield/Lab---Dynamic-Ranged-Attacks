using UnityEngine;

public class BowController : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public GameObject bowEquiped;

    private float launchSpeed = 50f;

    // Variables for pullback and bow properties
    private float currentPullback = 0f;
    private bool isPulling = false;
    private float drawbackStartTime;

    private float activeMultiplier = 1;
    private float activePullbackTime = 3;
    private float pullbackPercentage;

    private GameObject instantiatedArrow;
    private Vector3 arrowDirection = Vector3.forward;
    



private void Update()
    {
        if (isPulling)
        {
            UpdatePull();
            UpdateArrowDirection();
        }

        // Check for mouse button input
        if (Input.GetMouseButtonDown(0))
        {
            StartPulling();
        }
        else if (Input.GetMouseButtonUp(0) && isPulling)
        {
            ReleaseArrow();
        }
    }

    private void StartPulling()
    {
        // Bool to enable/disable logic when pulling back the bow
        isPulling = true;
        // Timestamp from when we started pulling
        drawbackStartTime = Time.time;

        // Instantiate the arrow when pulling starts
        instantiatedArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
    }

    private void UpdatePull() // Update the status of the bow pull
    {
        // How long have we been pulling? current time - the time stamp from when we started pulling
        float drawbackTime = Time.time - drawbackStartTime;
        // Clamp the pullback calculation to the max pullback time (activePullbackTime)
        currentPullback = Mathf.Clamp(drawbackTime, float.Epsilon, activePullbackTime);
        Debug.Log("currentPullback =" +  currentPullback);
        // Figure out what percentage of pullback we are currently at
        pullbackPercentage = Mathf.Clamp((currentPullback / activePullbackTime) * 100, float.Epsilon, 100);
        Debug.Log("pullbackPercentage =" + pullbackPercentage);
    }

    private void UpdateArrowDirection()
    {
        // Calculate the end point of the ray
        Vector3 rayEnd = arrowSpawnPoint.position + Camera.main.transform.forward * 10f;

        // Calculate the direction from the arrow spawn point to the end point of the ray
        arrowDirection = (rayEnd - arrowSpawnPoint.position).normalized;
        Debug.Log("While Updating arrowDirection is" + arrowDirection);

        // Debug draw the ray from the arrow spawn point to the end point
        Debug.DrawRay(arrowSpawnPoint.position, arrowDirection * 10f, Color.green);


        // Rotate arrow to face the calculated direction
        instantiatedArrow.transform.rotation = Quaternion.LookRotation(arrowDirection);

    }

    public void ReleaseArrow()
    {
        isPulling = false;

        // Ad an RB to the arrow and enable gravity for the arrow
        Rigidbody arrowRigidbody = instantiatedArrow.AddComponent<Rigidbody>();
        arrowRigidbody.useGravity = true;
        
        // Add force. Remap pullback% to 0 to 1, launchspd * pullback %, arrowDir * result
        arrowRigidbody.AddForce(arrowDirection * (launchSpeed * (pullbackPercentage / 100)), ForceMode.Impulse);
      
        // Send damage values to instantiated arrow via ArrowScript
        ArrowScript arrowScript = instantiatedArrow.GetComponent<ArrowScript>();
        float baseDamage = 1f; // Base damage of the arrow
        float totalDamage = baseDamage * activeMultiplier * pullbackPercentage;
        arrowScript.SetDamage(totalDamage);

        instantiatedArrow = null; // Reset instantiatedArrow reference
    }
}
