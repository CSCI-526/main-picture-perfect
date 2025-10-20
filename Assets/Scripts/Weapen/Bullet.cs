using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 60f;          // Speed of the bullet
    public float lifeTime = 2.0f;      // Bullet lifetime in seconds

    [Header("Freeze")]
    public float freezeDuration = 2.5f; // How long to freeze hit targets

    [Header("Move")]
    public float moveImpulse = 6f;

    [Header("Hit Filter")]
    public LayerMask hittableLayers = ~0;  // Layers that can be hit
    public bool destroyOnNonFreezable = true; // Whether to destroy bullet when hitting non-freezable objects

    private Rigidbody rb;
    private Collider col;
    private bool hasHit = false;
    private Transform ignoreRoot;  // Optional: Ignore collisions with shooterRoot

    // Optional: Ignore collisions with shooterRoot
    public void Initialize(Transform shooterRoot)
    {
        ignoreRoot = shooterRoot;
        if (ignoreRoot)
        {
            var myCol = GetComponent<Collider>();
            foreach (var c in ignoreRoot.GetComponentsInChildren<Collider>())
            {
                if (c.enabled) Physics.IgnoreCollision(myCol, c, true);
            }
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        col.isTrigger = true; // Use trigger to simplify collision handling
        col.enabled = true;

        transform.SetParent(null, true);
    }

    void OnEnable()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);

        transform.position += transform.forward * 0.01f;
        rb.velocity = transform.forward.normalized * speed;
    }

    void Start()
    {
        //rb.velocity = transform.forward.normalized * speed;
    }

    void FixedUpdate()
    {
        if (!hasHit && rb.velocity.sqrMagnitude < speed * speed * 0.5f)
        {
            rb.velocity = transform.forward.normalized * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (((1 << other.gameObject.layer) & hittableLayers) == 0)
            return;

        // Ignore collisions with shooterRoot
        if (ignoreRoot && other.transform.IsChildOf(ignoreRoot)) return;

        HandleHit(other);
    }

    private void HandleHit(Collider hitCol)
    {
        hasHit = true;

        // Get TargetBall
        var ball = hitCol.GetComponentInParent<TargetBall>();
        if (ball != null)
        {
            Debug.Log($"[Bullet] Hit TargetBall {ball.name}");
            ball.Freeze(freezeDuration, "hit"); // Freeze for specified duration, source = "hit"
            Destroy(gameObject);
            return;
        }


        var freezable = hitCol.GetComponentInParent<IFreezable>();
        if (freezable != null)
        {
            Debug.Log($"[Bullet] Freeze {((Component)freezable).gameObject.name} for {freezeDuration}s (hit {hitCol.name})");
            freezable.Freeze(freezeDuration, "hit"); // Freeze for specified duration, source = "hit"
            if (AnalyticsManager.Instance != null){
                if (hitCol.CompareTag("NPC"))
                {
                    AnalyticsManager.Instance.RecordNPCFrozen();
                }
                AnalyticsManager.Instance.RecordFreezeHit();
            }
            Destroy(gameObject);
            return;
        }


        var movable = hitCol.GetComponentInParent<IMovable>();
        if (movable != null)
        {
            Debug.Log($"[Bullet] Nudge movable: {((Component)movable).gameObject.name} (hit {hitCol.name})");
            movable.Nudge(moveImpulse);
            if (AnalyticsManager.Instance != null){
                AnalyticsManager.Instance.RecordMoveHit();
            }

            Destroy(gameObject);
            return;
        }

        // Hit something non-freezable
        if (destroyOnNonFreezable)
        {
            Debug.Log($"[Bullet] Hit non-freezable: {hitCol.name}");
            Destroy(gameObject);
        }
        else
        {
            hasHit = false; // Continue flying
        }
    }
}
