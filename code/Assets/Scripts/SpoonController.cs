using UnityEngine;

public class SpoonController : MonoBehaviour
{
    [Header("References")]
    public Bowl targetBowl;

    [Header("State")]
    public bool hasFood;

    [Header("Scoop Settings")]
    public float scoopDistance = 0.35f; // How close spoon needs to be to bowl to scoop
    public Transform eatPosition;      // Assign Main Camera transform here

    [Header("Eat Settings")]
    public float eatDistance = 0.5f;   // How close spoon needs to be to camera to eat

    private bool isScooping;
    private bool isEating;
    private MouseDragInteraction dragScript;

    private void Start()
    {
        dragScript = GetComponent<MouseDragInteraction>();
        if (targetBowl == null && GameManager.Instance != null)
            targetBowl = GameManager.Instance.bowl;
        if (eatPosition == null)
            eatPosition = Camera.main.transform;
    }

    private void OnMouseDown()
    {
        // Just picking up the spoon — give the player a hint about what to do next
        if (!hasFood)
            GameManager.Instance.ShowMessage("Drag the spoon over the bowl, then release to scoop.");
        else
            GameManager.Instance.ShowMessage("Drag the spoon toward the camera (scroll to bring it closer), then release to eat.");
    }

    private void OnMouseUp()
    {
        if (!hasFood)
        {
            // Released spoon — check if it landed near the bowl
            TryScoop();
        }
        else
        {
            // Already holding food — check if it's near the camera to eat
            TryEat();
        }
    }

    private void TryScoop()
    {
        if (targetBowl == null) return;

        float dist = Vector3.Distance(transform.position, targetBowl.transform.position);

        if (dist > scoopDistance)
        {
            GameManager.Instance.ShowMessage("Move the spoon closer to the bowl! (Drag it near the bowl, then click)");
            return;
        }

        if (!targetBowl.IsReadyToEat() &&
            targetBowl.currentState != Bowl.BowlState.PartiallyEaten)
        {
            if (targetBowl.currentState == Bowl.BowlState.Empty ||
                targetBowl.currentState == Bowl.BowlState.Finished)
            {
                GameManager.Instance.ShowMessage("The bowl is empty! Add cereal and milk first.");
            }
            else if (targetBowl.currentState == Bowl.BowlState.HasCereal)
            {
                GameManager.Instance.ShowMessage("Add milk to the bowl before eating!");
            }
            else if (targetBowl.currentState == Bowl.BowlState.HasMilk)
            {
                GameManager.Instance.ShowMessage("Add cereal to the bowl before eating!");
            }
            return;
        }

        // Successfully scooped
        hasFood = true;
        GameManager.Instance.ShowMessage("Scooped!  Now drag the spoon toward the camera then release the mouse button to eat.");

        // Visual feedback: change spoon color
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = new Color(1f, 0.9f, 0.6f);
    }

    private void TryEat()
    {
        if (!hasFood) return;

        if (eatPosition == null) return;

        float dist = Vector3.Distance(transform.position, eatPosition.position);

        if (dist <= eatDistance)
        {
            // Eating!
            hasFood = false;
            targetBowl.TakeBite();

            // Reset spoon color
            Renderer rend = GetComponentInChildren<Renderer>();
            if (rend != null)
                rend.material.color = Color.white;
        }
        else
        {
            GameManager.Instance.ShowMessage("The spoon is not close enough to eat! Drag it nearer to the camera, then release.");
        }
    }
}
