using UnityEngine;

[RequireComponent(typeof(RespawnManager))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 3;
    private int currentHits = 0;

    private RespawnManager respawn;

    void Awake()
    {
        respawn = GetComponent<RespawnManager>();
    }

    public void TakeDamage(int amount)
    {
        currentHits += amount;
        Debug.Log($"Player hit {currentHits}/{maxHits}");

        if (currentHits >= maxHits)
        {
            Respawn();
        }
    }

    public void RestoreFullHealth()
    {//  for collectable
        currentHits = 0;
        Debug.Log("Health restored to full!");
    }


    void Respawn()
    {
        currentHits = 0;
        respawn.Respawn();
    }
}
