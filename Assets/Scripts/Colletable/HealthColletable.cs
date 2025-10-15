using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    public float rotateSpeed = 90f;

    private Vector3 startPos;
    private Quaternion startRot;
    private bool collected = false;
    private Renderer rend;
    private Collider coll;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        rend = GetComponent<Renderer>();
        coll = GetComponent<Collider>();
    }

    void Update()
    {
        if (!collected)
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null && !hp.IsFullHealth())
            {
                hp.RestoreFullHealth();
                collected = true;
                rend.enabled = false;
                coll.enabled = false;
            }
        }
    }

    public void Respawn()
    {
        collected = false;
        transform.position = startPos;
        transform.rotation = startRot;
        rend.enabled = true;
        coll.enabled = true;
    }
}
