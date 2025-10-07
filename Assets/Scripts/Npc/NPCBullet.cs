using UnityEngine;
  

public class NPCBullet : MonoBehaviour
{
    private PlayerHealth target;
    public float lifeTime = 3f;
    public float hitRadius = 0.4f;
    public LayerMask playerMask;  

    public void Init(PlayerHealth playerHealth)
    {
        target = playerHealth;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        
        if (target != null)
        {
            Vector3 start = transform.position;
            Vector3 end = transform.position + transform.forward * 0.1f;
            
            if (Physics.CheckCapsule(start, end, hitRadius, LayerMask.GetMask("Player")))
            {
                target.TakeDamage(1);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
