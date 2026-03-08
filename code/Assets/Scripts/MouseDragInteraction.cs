using UnityEngine;

public class MouseDragInteraction : MonoBehaviour
{
    private Camera mainCam;
    private bool isDragging;
    private float dragDepth;
    private Rigidbody rb;
    private bool isPlaced;

    [HideInInspector] public bool canDrag = true;

    [Header("Depth Settings")]
    public float scrollSpeed = 0.5f;
    public float minDepth = 0.5f;
    public float maxDepth = 10f;

    private void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (!canDrag) return;

        isDragging = true;
        dragDepth = mainCam.WorldToScreenPoint(transform.position).z;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging || !canDrag) return;

        // Scroll wheel adjusts how far the object is from the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        dragDepth += scroll * scrollSpeed;
        dragDepth = Mathf.Clamp(dragDepth, minDepth, maxDepth);

        Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    public void LockInPlace(Vector3 position)
    {
        isDragging = false;
        isPlaced = true;
        canDrag = false;
        transform.position = position;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
