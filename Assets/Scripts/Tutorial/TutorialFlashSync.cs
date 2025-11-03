using UnityEngine;

public class TutorialFlashSync : MonoBehaviour
{
    
    public Renderer gunRenderer;
    public Freezable target;

    
    public Color flashColor = Color.red;
    public float flashSpeed = 2f;

    private Renderer targetRenderer;
    private Color gunBaseColor;
    private Color targetBaseColor;
    private float t;
    private bool tutorialComplete = false;

    void Start()
    {
        if (gunRenderer == null)
            gunRenderer = GetComponentInChildren<Renderer>();

        if (target != null)
            targetRenderer = target.GetComponentInChildren<Renderer>();

        
        if (gunRenderer != null)
        {
            gunRenderer.material = new Material(gunRenderer.material);
            gunBaseColor = gunRenderer.material.color;
        }

        if (targetRenderer != null)
        {
            targetRenderer.material = new Material(targetRenderer.material);
            targetBaseColor = targetRenderer.material.color;
        }
    }

    void Update()
    {
        if (target == null || targetRenderer == null || gunRenderer == null)
            return;

        if (target.IsFrozen && !tutorialComplete)
        {
            tutorialComplete = true;

           
            gunRenderer.material.color = gunBaseColor;
            targetRenderer.material.color = targetBaseColor;

           
        }

        
        if (!tutorialComplete)
        {
            t += Time.deltaTime * flashSpeed;
            float lerpValue = (Mathf.Sin(t) + 1f) / 2f;
            Color newColor = Color.Lerp(targetBaseColor, flashColor, lerpValue);

            gunRenderer.material.color = newColor;
            targetRenderer.material.color = newColor;
        }
    }
}
