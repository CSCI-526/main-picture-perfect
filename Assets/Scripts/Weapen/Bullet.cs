using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 60f;          // 初速度
    public float lifeTime = 2.0f;      // 存活时间

    [Header("Freeze")]
    public float freezeDuration = 2.5f; // 命中可冻结目标时的冻结时长

    [Header("Hit Filter")]
    public LayerMask hittableLayers = ~0;  // 允许命中的图层（建议只勾选放小球或平台的层）
    public bool destroyOnNonFreezable = true; // 打到不可冻结物体是否也销毁子弹

    private Rigidbody rb;
    private Collider col;
    private bool hasHit = false;
    private Transform ignoreRoot;  // 可选：忽略与发射者自身的碰撞

    // 初始化时可传入玩家 Transform，用于忽略与自身的碰撞
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

        col.isTrigger = true; // 用触发检测命中
        col.enabled = true;
    }

    void OnEnable()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    void Start()
    {
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // 图层过滤：不在 hittableLayers 的直接忽略
        if (((1 << other.gameObject.layer) & hittableLayers) == 0)
            return;

        // 避开发射者本体
        if (ignoreRoot && other.transform.IsChildOf(ignoreRoot)) return;

        HandleHit(other);
    }

    private void HandleHit(Collider hitCol)
    {
        hasHit = true;

        // 1. 命中小球（TargetBall）
        var ball = hitCol.GetComponentInParent<TargetBall>();
        if (ball != null)
        {
            Debug.Log($"[Bullet] Hit TargetBall {ball.name}");
            ball.Freeze(freezeDuration); // 直接调用 Freeze（TargetBall 自己控制消失/恢复）
            Destroy(gameObject);
            return;
        }

        // 2. 命中其他 Freezable
        var freezable = hitCol.GetComponentInParent<IFreezable>();
        if (freezable != null)
        {
            Debug.Log($"[Bullet] Freeze {((Component)freezable).gameObject.name} for {freezeDuration}s (hit {hitCol.name})");
            freezable.Freeze(freezeDuration);
            Destroy(gameObject);
            return;
        }

        // 3. 命中不可冻结的：按开关决定是否销毁
        if (destroyOnNonFreezable)
        {
            Debug.Log($"[Bullet] Hit non-freezable: {hitCol.name}");
            Destroy(gameObject);
        }
        else
        {
            hasHit = false; // 允许继续命中下一个
        }
    }
}
