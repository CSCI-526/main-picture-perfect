using UnityEngine;

public class SetRespawnPointOnTouch : MonoBehaviour
{
    [Tooltip("Vertical offset for the respawn point above the platform")]
    public float yOffset = 5f;

    private bool hasTriggered = false;  // whether the respawn point has been set

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;  // If already triggered, do nothing

        if (other.CompareTag("Player"))
        {
            RespawnManager respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager != null)
            {
                GameObject tempSpawn = new GameObject("TempSpawnPoint");
                Vector3 platformPos = transform.position;
                tempSpawn.transform.position = platformPos + Vector3.up * yOffset;

                respawnManager.SetSpawn(tempSpawn.transform);
                Debug.Log($" Respawn point set to: {tempSpawn.transform.position}");

                hasTriggered = true;  // change state to triggered
            }
            else
            {
                Debug.LogWarning("RespawnManager not found.");
            }
        }
    }
}
