using UnityEngine;

public class SetRespawnPointOnTouch : MonoBehaviour
{
    [Tooltip("设置重生点时，在平台上方的垂直偏移量")]
    public float yOffset = 5f;

    private bool hasTriggered = false;  // 是否已触发过

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;  // 已触发过则忽略

        if (other.CompareTag("Player"))
        {
            RespawnManager respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager != null)
            {
                GameObject tempSpawn = new GameObject("TempSpawnPoint");
                Vector3 platformPos = transform.position;
                tempSpawn.transform.position = platformPos + Vector3.up * yOffset;

                respawnManager.SetSpawn(tempSpawn.transform);
                Debug.Log($"✅ Respawn point set to: {tempSpawn.transform.position}");

                hasTriggered = true;  // 标记为已触发
            }
            else
            {
                Debug.LogWarning("⚠️ RespawnManager not found.");
            }
        }
    }
}
