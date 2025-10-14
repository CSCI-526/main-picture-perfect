using UnityEngine;

[RequireComponent(typeof(RespawnManager))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 3;
    private int currentHits = 0;

    private RespawnManager respawn;
    private HealthBarUI healthBar;
    private bool isInvincible = false;

    void Awake()
    {
        respawn = GetComponent<RespawnManager>();
        healthBar = FindObjectOfType<HealthBarUI>();

        if (healthBar != null)
        {
            healthBar.InitializeHearts(maxHits);
            healthBar.UpdateHearts(currentHits, maxHits);
        }

        StartCoroutine(StartInvincibility(5f)); //no health loss for 5 s
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHits = Mathf.Clamp(currentHits + amount, 0, maxHits);
       

        if (healthBar != null)
            healthBar.UpdateHearts(currentHits, maxHits);

        if (currentHits >= maxHits)
            Respawn();
    }

    public void RestoreFullHealth()
    {
        currentHits = 0;
        
        if (healthBar != null)
            healthBar.UpdateHearts(currentHits, maxHits);
    }

    void Respawn()
    {
        currentHits = 0;
        respawn.Respawn();
        if (healthBar != null)
            healthBar.UpdateHearts(currentHits, maxHits);

        StartCoroutine(StartInvincibility(5f)); //5 sec no loss health
    }

    private System.Collections.IEnumerator StartInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log($"Invincible for {duration} seconds");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Invincibility ended");
    }
}
