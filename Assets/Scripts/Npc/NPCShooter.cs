using System.Collections;
using UnityEngine;

public class NPCShooter : Freezable
{
    
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform player;

   
    public float fireRate = 1.2f;
    public float bulletSpeed = 40f;
    public float attackRange = 20f;   // Only shoot when player is within this distance
    public float lookRange = 25f;     // Still turn to face player within this distance

    private float nextFireTime = 0f;

    protected override void Update()
    {
        // Handle freeze countdown
        base.Update();

        if (player == null || IsFrozen) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Face the player if within look range
        if (dist <= lookRange)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Shoot only if within attack range
        if (dist <= attackRange && Time.time >= nextFireTime)
        {
            FireAtPlayer();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireAtPlayer()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 aim = (player.position + Vector3.up * 1.2f - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(aim);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = aim * bulletSpeed;

        NPCBullet b = bullet.AddComponent<NPCBullet>();
        b.Init(player.GetComponent<PlayerHealth>());
    }

    protected override void OnFreeze()
    {
        Debug.Log($"{name} is frozen and stops shooting.");
    }

    protected override void OnUnfreeze()
    {
        Debug.Log($"{name} unfrozen and resumes shooting.");
    }
}
