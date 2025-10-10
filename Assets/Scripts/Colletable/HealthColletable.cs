using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    public float rotateSpeed = 90f; // rotation speed for visual effect

    void Update()
    {
        // spin for visibility
        transform.Rotate(Vector3.up* rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.RestoreFullHealth();
            }

            Destroy(gameObject); // remove collectable after use
        }
    }
}
