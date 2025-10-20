using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public TMP_Text ammoText;

    [Header("Gun Settings")]
    public int magazineSize = 5;
    public float reloadTime = 1.667f;
    public float fireRate = 0.1f;

    private int currentAmmo;
    private bool isReloading = false;
    private float fireCooldown = 0f;
    private Animator animator;

    struct ShotReq
    {
        public Vector3 pos;
        public Quaternion rot;
        public Transform shooterRoot;
    }
    readonly Queue<ShotReq> shotQueue = new Queue<ShotReq>();

    void Start()
    {
        currentAmmo = magazineSize;
        UpdateAmmoUI();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // If currently reloading, skip firing
        if (isReloading) return;

        fireCooldown -= Time.deltaTime;

        // Fire
        if (Input.GetButton("Fire1") && fireCooldown <= 0f && currentAmmo > 0)
        {
            fireCooldown = fireRate;
            currentAmmo--;
            UpdateAmmoUI();
            FireAnimation();
            shotQueue.Enqueue(new ShotReq {
                pos = firePoint.position,
                rot = firePoint.rotation,
                shooterRoot = transform.root
            });
            AnalyticsManager.Instance.RecordBulletFired();

        }

        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Auto reload when ammo is empty
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void FixedUpdate()
    {
        while (shotQueue.Count > 0)
        {
            var req = shotQueue.Dequeue();
            var go = Instantiate(bulletPrefab, req.pos, req.rot); 
            var bullet = go.GetComponent<Bullet>();
            if (bullet) bullet.Initialize(req.shooterRoot);      
        }
    }

    void FireAnimation()
    {
        if (animator)
        {
            animator.SetBool("isFiring", true);
            CancelInvoke(nameof(StopFireAnimation));
            Invoke(nameof(StopFireAnimation), fireRate * 0.9f); // Keep true for a short time
        }
    }

    void StopFireAnimation()
    {
        if (animator)
            animator.SetBool("isFiring", false);
    }

    IEnumerator Reload()
    {
        // Prevent multiple reloads at the same time
        if (isReloading) yield break;
        isReloading = true;

        // Update UI text
        if (ammoText) ammoText.text = "Reloading...";

        // Play reload animation
        animator?.SetBool("isReloading", true);

        // Wait one frame to ensure animator state is updated
        yield return null;

        // Default duration (fallback)
        float reloadDuration = reloadTime;

        // Try to get the actual duration of the current reload animation
        if (animator != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                reloadDuration = clipInfo[0].clip.length;
                // Debug.Log($"Reload animation length: {reloadDuration:F2} seconds");
            }
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(reloadDuration);

        // Restore weapon state after reloading
        currentAmmo = magazineSize;
        UpdateAmmoUI();

        animator?.SetBool("isReloading", false);
        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText)
            ammoText.text = $"{currentAmmo} / {magazineSize}";
    }
}
