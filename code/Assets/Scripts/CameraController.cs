using UnityEngine;

/// <summary>
/// Simple camera controller for mouse-based interaction.
/// Right-click + drag to rotate the camera around the table.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform lookTarget;         // Assign the table or bowl target
    public float rotateSpeed = 3f;

    private float currentDistance;
    private float yaw;
    private float pitch;

    private void Start()
    {
        if (lookTarget == null)
        {
            // Default: look at origin (table center)
            GameObject target = new GameObject("CameraTarget");
            target.transform.position = new Vector3(0, 0.8f, 0);
            lookTarget = target.transform;
        }

        Vector3 offset = transform.position - lookTarget.position;
        currentDistance = offset.magnitude;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void LateUpdate()
    {
        // Right-click drag to orbit
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotateSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotateSpeed;
            pitch = Mathf.Clamp(pitch, 5f, 80f);
        }

        // Apply rotation and position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = lookTarget.position - rotation * Vector3.forward * currentDistance;

        transform.rotation = rotation;
        transform.position = position;
    }
}
