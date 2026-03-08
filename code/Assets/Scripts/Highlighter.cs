using UnityEngine;

/// <summary>
/// Highlights an object when the mouse hovers over it.
/// Requires a Collider on the object.
/// </summary>
public class Highlighter : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color hoverColor = Color.yellow;
    private bool isHighlighted;

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    private void OnMouseEnter()
    {
        if (rend != null)
        {
            rend.material.color = Color.Lerp(originalColor, hoverColor, 0.3f);
            isHighlighted = true;
        }
    }

    private void OnMouseExit()
    {
        if (rend != null && isHighlighted)
        {
            rend.material.color = originalColor;
            isHighlighted = false;
        }
    }

    // Call this when the base color changes (e.g., bowl state change)
    public void UpdateBaseColor()
    {
        if (rend != null)
            originalColor = rend.material.color;
    }
}
