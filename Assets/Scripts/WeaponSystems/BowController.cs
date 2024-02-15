using UnityEngine;

public class BowController : MonoBehaviour
{
    public GameObject arrowPrefab; // The arrow prefab we will be firing
    public Transform arrowSpawnPoint; // The location we spawn the arrow in
    public GameObject bowEquiped; // The bow prefab we have equiped
    private BowType bowType; // A script holding the attributes of the bow
    private string bowName; // The name of the bow from it's attributes

    private float launchSpeed = 50f; // The speed attrivute that arrows are fired

    // Variables for pullback and bow properties
    private float currentPullback = 0f; // How long we have been pulling back the bow (clamped to max value)
    private bool isPulling = false; // Are we pulling the bow?
    private float drawbackStartTime; // When did we start pulling back the bow?

    private float activeMultiplier; // The multiplier attribute for the equiped bow
    private float activePullbackTime; // The pullback time for the equiped bow
    private float pullbackPercentage; // What percentage of pullback we currently are at

    private GameObject instantiatedArrow; // The instance of the arrow to be fired
    private Vector3 arrowDirection = Vector3.forward; // The direction the arrow is facing


    private void Start()
    {
        EquipBow();
    }

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

    private void EquipBow()
    {
        // Get the BowType script attached to the equipped bow
        if (bowEquiped != null)
        {
            bowType = bowEquiped.GetComponent<BowType>();
            Debug.Log("Bow Name = " + bowName);
            if (bowType != null)
            {
                // Set activeMultiplier and activePullbackTime from the BowType script
                activeMultiplier = bowType.damageMultiplier;
                activePullbackTime = bowType.pullbackTime;
                bowName = bowType.typeName;
            }
            else
            {
                Debug.LogError("No BowType script found on the equipped bow.");
            }
        }
        else
        {
            Debug.LogError("No equipped bow GameObject assigned to the bowEquiped field.");
        }
    }

    private void StartPulling()
    {
        isPulling = true;
        // record when we started pulling
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
       
        // Figure out what percentage of pullback we are currently at
        pullbackPercentage = Mathf.Clamp((currentPullback / activePullbackTime) * 100, float.Epsilon, 100);
     
    }

    private void UpdateArrowDirection()
    {
        // Calculate the end point of the ray
        Vector3 rayEnd = arrowSpawnPoint.position + Camera.main.transform.forward * 10f;

        // Calculate the direction from the arrow spawn point to the end point of the ray
        arrowDirection = (rayEnd - arrowSpawnPoint.position).normalized;

        // Debug draw the ray from the arrow spawn point to the end point
        Debug.DrawRay(arrowSpawnPoint.position, arrowDirection * 10f, Color.green);

        // Rotate arrow to face the calculated direction
        instantiatedArrow.transform.rotation = Quaternion.LookRotation(arrowDirection);

    }

    public void ReleaseArrow()
    {
        isPulling = false;

        // Ad a rigidbody to the arrow and enable gravity for the arrow
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
