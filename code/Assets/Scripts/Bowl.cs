using UnityEngine;

public class Bowl : MonoBehaviour
{
    public enum BowlState
    {
        Empty,
        HasCereal,
        HasMilk,
        Full,         // Has both cereal and milk - Ready to Eat
        PartiallyEaten,
        Finished      // Returns to empty after fully eaten
    }

    [Header("State")]
    public BowlState currentState = BowlState.Empty;
    public bool hasCereal;
    public bool hasMilk;
    public bool isPlacedOnTarget;

    [Header("Visual Meshes - Assign in Inspector")]
    public GameObject emptyVisual;          // Default bowl mesh (always visible as base)
    public GameObject cerealVisual;         // Cereal inside bowl
    public GameObject milkVisual;           // Milk inside bowl
    public GameObject fullVisual;           // Combined cereal+milk mesh
    public GameObject partiallyEatenVisual; // Partially eaten mesh

    [Header("Eating")]
    public int totalBites = 3;
    private int bitesRemaining;

    private void Start()
    {
        bitesRemaining = totalBites;
        UpdateVisuals();
    }

    public void AddCereal()
    {
        if (hasCereal)
        {
            GameManager.Instance.ShowMessage("Bowl already has cereal!");
            return;
        }

        if (!isPlacedOnTarget)
        {
            GameManager.Instance.ShowMessage("Place the bowl on the table first!");
            return;
        }

        hasCereal = true;
        UpdateState();
        GameManager.Instance.ShowMessage("Cereal added to the bowl!");
    }

    public void AddMilk()
    {
        if (hasMilk)
        {
            GameManager.Instance.ShowMessage("Bowl already has milk!");
            return;
        }

        if (!isPlacedOnTarget)
        {
            GameManager.Instance.ShowMessage("Place the bowl on the prep zone first!");
            return;
        }

        hasMilk = true;
        UpdateState();
        GameManager.Instance.ShowMessage("Milk added to the bowl!");
    }

    public bool IsReadyToEat()
    {
        return currentState == BowlState.Full;
    }

    public bool TakeBite()
    {
        if (currentState == BowlState.Full || currentState == BowlState.PartiallyEaten)
        {
            bitesRemaining--;

            if (bitesRemaining <= 0)
            {
                currentState = BowlState.Finished;
                hasCereal = false;
                hasMilk = false;
                GameManager.Instance.ShowMessage("Bowl is now empty! Breakfast complete!");
            }
            else
            {
                currentState = BowlState.PartiallyEaten;
                GameManager.Instance.ShowMessage("Yum! " + bitesRemaining + " bites remaining.");
            }

            UpdateVisuals();
            return true;
        }

        return false;
    }

    private void UpdateState()
    {
        if (hasCereal && hasMilk)
        {
            currentState = BowlState.Full;
            bitesRemaining = totalBites;
            GameManager.Instance.ShowMessage("Bowl is full! Ready to eat!");
        }
        else if (hasCereal)
            currentState = BowlState.HasCereal;
        else if (hasMilk)
            currentState = BowlState.HasMilk;
        else
            currentState = BowlState.Empty;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Safely toggle each visual if assigned
        if (cerealVisual != null)
            cerealVisual.SetActive(currentState == BowlState.HasCereal);
        if (milkVisual != null)
            milkVisual.SetActive(currentState == BowlState.HasMilk);
        if (fullVisual != null)
            fullVisual.SetActive(currentState == BowlState.Full);
        if (partiallyEatenVisual != null)
            partiallyEatenVisual.SetActive(currentState == BowlState.PartiallyEaten);

        // Change bowl color as fallback visual feedback
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            switch (currentState)
            {
                case BowlState.Empty:
                case BowlState.Finished:
                    rend.material.color = Color.white;
                    break;
                case BowlState.HasCereal:
                    rend.material.color = new Color(1f, 0.9f, 0.5f); // Light yellow
                    break;
                case BowlState.HasMilk:
                    rend.material.color = new Color(0.9f, 0.95f, 1f); // Light blue-white
                    break;
                case BowlState.Full:
                    rend.material.color = new Color(1f, 0.85f, 0.6f); // Orange-ish
                    break;
                case BowlState.PartiallyEaten:
                    rend.material.color = new Color(1f, 0.9f, 0.7f); // Lighter
                    break;
            }
        }
    }
}
