using UnityEngine;

public class TargetZone : MonoBehaviour
{
    [Header("Settings")]
    public Bowl bowl;
    public Vector3 snapOffset = Vector3.zero; // Fine-tune snap position

    [Header("Visual")]
    public Color highlightColor = new Color(0f, 1f, 0f, 0.3f);
    public Color defaultColor = new Color(1f, 1f, 0f, 0.2f);

    private Renderer zoneRenderer;
    private bool bowlPlaced;

    private void Start()
    {
        zoneRenderer = GetComponent<Renderer>();
        if (zoneRenderer != null)
        {
            // Make it semi-transparent
            Material mat = zoneRenderer.material;
            mat.color = defaultColor;
            SetTransparent(mat);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bowlPlaced) return;

        Bowl incomingBowl = other.GetComponent<Bowl>();
        if (incomingBowl != null)
        {
            // Highlight the zone
            if (zoneRenderer != null)
                zoneRenderer.material.color = highlightColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (bowlPlaced) return;

        Bowl incomingBowl = other.GetComponent<Bowl>();
        if (incomingBowl == null) return;

        // Check if player released the mouse (stopped dragging)
        if (!Input.GetMouseButton(0))
        {
            PlaceBowl(incomingBowl);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (bowlPlaced) return;

        Bowl incomingBowl = other.GetComponent<Bowl>();
        if (incomingBowl != null && zoneRenderer != null)
        {
            zoneRenderer.material.color = defaultColor;
        }
    }

    private void PlaceBowl(Bowl incomingBowl)
    {
        bowlPlaced = true;
        incomingBowl.isPlacedOnTarget = true;

        // Snap bowl to target position
        Vector3 snapPos = transform.position + snapOffset;
        MouseDragInteraction drag = incomingBowl.GetComponent<MouseDragInteraction>();
        if (drag != null)
        {
            drag.LockInPlace(snapPos);
        }
        else
        {
            incomingBowl.transform.position = snapPos;
        }

        // Change zone color to indicate success
        if (zoneRenderer != null)
            zoneRenderer.material.color = new Color(0f, 1f, 0f, 0.5f);

        GameManager.Instance.ShowMessage("Bowl placed! Now click cereal box or milk carton to pour.");
    }

    private void SetTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
