using UnityEngine;

public class TrajectoryVisualizer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int resolution = 20; // Number of points along the trajectory curve
    public float maxTime = 2f; // Maximum time to predict trajectory
    public GameObject arrowPrefab; // Prefab of the arrow

    private GameObject tempArrow; // Temporary arrow object for trajectory calculation

    private void Update()
    {
        // Instantiate a temporary arrow when aiming begins
        if (Input.GetMouseButtonDown(0)) // Assuming right mouse button initiates aiming
        {
            tempArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        }
        // Destroy the temporary arrow when aiming ends
        else if (Input.GetMouseButtonUp(0)) // Assuming right mouse button ends aiming
        {
            Destroy(tempArrow);
        }

        // Visualize trajectory when aiming
        if (tempArrow != null)
        {
            Vector3 startPosition = tempArrow.transform.position;
            Quaternion startRotation = tempArrow.transform.rotation;
            Rigidbody tempArrowRigidbody = tempArrow.GetComponent<Rigidbody>();

            // Calculate initial velocity vector
            Vector3 initialVelocity = tempArrowRigidbody.velocity;

            // Calculate trajectory points
            Vector3[] trajectoryPoints = CalculateTrajectoryPoints(startPosition, initialVelocity);

            // Draw trajectory line
            DrawTrajectoryLine(trajectoryPoints);
        }
    }

    // Calculate trajectory points using kinematic equations
    private Vector3[] CalculateTrajectoryPoints(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] trajectoryPoints = new Vector3[resolution];
        float timeStep = maxTime / (resolution - 1);

        for (int i = 0; i < resolution; i++)
        {
            float currentTime = i * timeStep;
            trajectoryPoints[i] = startPosition + initialVelocity * currentTime + 0.5f * Physics.gravity * currentTime * currentTime;
        }

        return trajectoryPoints;
    }

    // Draw trajectory line using Line Renderer
    private void DrawTrajectoryLine(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
}
