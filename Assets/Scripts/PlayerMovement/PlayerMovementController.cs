using UnityEngine;
using Cinemachine;

public class PlayerMovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxSpeedMultiplier = 3f;
    public float currentSpeedMultiplier = 1f;
    public float cameraSensitivity = 2f;
    public float maxVerticalAngle = 80f; // Maximum vertical angle for looking up and down
    public RectTransform canvasRect; // Reference to the canvas RectTransform

    public CinemachineVirtualCamera thirdPersonCamera;
    public CinemachineVirtualCamera aimCamera;

    private bool isAiming = false;
    private Transform playerTransform;
    private float verticalRotation = 0f;

    private void Start()
    {
        playerTransform = transform;
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor at the center of the screen
        Cursor.visible = false; // Hide cursor
        thirdPersonCamera.gameObject.SetActive(true);
        aimCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Player movement with WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // moveSpeed multiplier when holding shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeedMultiplier = maxSpeedMultiplier;
        }
        else
        {
            currentSpeedMultiplier = 1;
        }
            Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        playerTransform.Translate(movement * moveSpeed * currentSpeedMultiplier * Time.deltaTime);

        // Rotate the player around the y-axis based on horizontal mouse movement
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        playerTransform.Rotate(Vector3.up, mouseX);

        // Toggle aiming mode with right mouse button
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            isAiming = true;
            thirdPersonCamera.gameObject.SetActive(false);
            aimCamera.gameObject.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            aimCamera.gameObject.SetActive(false);
            thirdPersonCamera.gameObject.SetActive(true);
        }

        // Rotate the camera based on vertical mouse movement when aiming
        if (isAiming)
        {
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * -1; // Invert vertical aiming
            // Calculate the new vertical rotation
            verticalRotation += mouseY;
            // Clamp the vertical rotation within the specified limits
            verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);
            // Apply the rotation to the aim camera
            aimCamera.transform.localRotation = Quaternion.Euler(verticalRotation, aimCamera.transform.localEulerAngles.y, 0f);
        }
        else // Rotate the third person camera if not aiming
        {
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * -1; // Invert vertical aiming
            // Calculate the new vertical rotation
            verticalRotation += mouseY;
            // Clamp the vertical rotation within the specified limits
            verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);
            // Apply the rotation to the third person camera
            thirdPersonCamera.transform.localRotation = Quaternion.Euler(verticalRotation, thirdPersonCamera.transform.localEulerAngles.y, 0f);
        }

        // Calculate the rotation based on the mouse position within the canvas bounds
        if (canvasRect != null)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 canvasSize = canvasRect.sizeDelta;
            float xRotation = (mousePos.y / canvasSize.y) * maxVerticalAngle * 2f - maxVerticalAngle; // Map mouse y position to vertical rotation
            aimCamera.transform.localRotation = Quaternion.Euler(xRotation, aimCamera.transform.localEulerAngles.y, 0f);
        }
    }
}
