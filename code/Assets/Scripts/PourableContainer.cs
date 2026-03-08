using UnityEngine;

public class PourableContainer : MonoBehaviour
{
    public enum ContainerType
    {
        CerealBox,
        MilkCarton
    }

    [Header("Settings")]
    public ContainerType containerType;
    public Bowl targetBowl;

    [Header("Pour Particle (Optional)")]
    public ParticleSystem pourParticles;

    [Header("Tilt Animation")]
    public float tiltAngle = 45f;
    public float tiltSpeed = 3f;

    private bool isPouring;
    private bool hasPoured;
    private Quaternion originalRotation;
    private Quaternion tiltedRotation;
    private float pourTimer;
    private float pourDuration = 1.5f;

    private void Start()
    {
        originalRotation = transform.rotation;
        tiltedRotation = originalRotation * Quaternion.Euler(0, 0, tiltAngle);
    }

    // Click to pour - simplified from VR tilt
    private void OnMouseDown()
    {
        if (hasPoured) return;

        if (targetBowl == null)
        {
            targetBowl = GameManager.Instance.bowl;
        }

        if (targetBowl == null || !targetBowl.isPlacedOnTarget)
        {
            GameManager.Instance.ShowMessage("Place the bowl on the table first!");
            return;
        }

        // Check if this ingredient was already added
        if (containerType == ContainerType.CerealBox && targetBowl.hasCereal)
        {
            GameManager.Instance.ShowMessage("Bowl already has cereal!");
            return;
        }
        if (containerType == ContainerType.MilkCarton && targetBowl.hasMilk)
        {
            GameManager.Instance.ShowMessage("Bowl already has milk!");
            return;
        }

        isPouring = true;
        pourTimer = 0f;

        if (pourParticles != null)
            pourParticles.Play();
    }

    private void Update()
    {
        if (isPouring)
        {
            pourTimer += Time.deltaTime;

            // Animate tilt
            float t = Mathf.Clamp01(pourTimer / (pourDuration * 0.5f));
            if (pourTimer < pourDuration * 0.5f)
            {
                // Tilt forward
                transform.rotation = Quaternion.Lerp(originalRotation, tiltedRotation, t);
            }
            else
            {
                // Tilt back
                float t2 = Mathf.Clamp01((pourTimer - pourDuration * 0.5f) / (pourDuration * 0.5f));
                transform.rotation = Quaternion.Lerp(tiltedRotation, originalRotation, t2);
            }

            // Finish pouring
            if (pourTimer >= pourDuration)
            {
                isPouring = false;
                hasPoured = true;
                transform.rotation = originalRotation;

                if (pourParticles != null)
                    pourParticles.Stop();

                // Add contents to bowl
                if (containerType == ContainerType.CerealBox)
                    targetBowl.AddCereal();
                else
                    targetBowl.AddMilk();
            }
        }
    }

    // Allow pouring again (e.g., after bowl is emptied)
    public void ResetPour()
    {
        hasPoured = false;
    }
}
