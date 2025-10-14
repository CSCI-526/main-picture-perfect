using UnityEngine;

[RequireComponent(typeof(RespawnManager))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 3;
    private int currentHits = 0;

    private RespawnManager respawn;
    private HealthBarUI healthBar;

    void Awake()
    {
        respawn = GetComponent<RespawnManager>();
        healthBar = FindObjectOfType<HealthBarUI>();

        if (healthBar != null)
        {
            healthBar.InitializeHearts(maxHits);
            healthBar.UpdateHearts(currentHits, maxHits);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHits = Mathf.Clamp(currentHits + amount, 0, maxHits);
        Debug.Log($"Player hit {currentHits}/{maxHits}");

        if (healthBar != null)
            healthBar.UpdateHearts(currentHits, maxHits);

        if (currentHits >= maxHits)
            Respawn();
    }

    public void RestoreFullHealth()
    {
        currentHits = 0;
        Debug.Log("Health restored to full!");
        if (healthBar != null)
            healthBar.UpdateHearts(currentHits, maxHits);
    }

    void Respawn()
    {
        currentHits = 0;
        respawn.Respawn();
        if (healthBar != null)
            healthBar.UpdateHearts(currentHits, maxHits);
    }
}
