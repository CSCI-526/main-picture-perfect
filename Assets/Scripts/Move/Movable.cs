using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movable : MonoBehaviour, IMovable
{
    public enum MoveMode
    {
        PingPongToCorner,   //Origin -> End -> Origin (repeat N cycles)
        ImpulseSingleAxis   //One-off shove along Axis 0 (no auto return)
    }

    [Header("Mode")]
    [SerializeField] MoveMode mode = MoveMode.PingPongToCorner;

    [Header("Axis Space")]
    [Tooltip("If ON, axes are relative to this transform's rotation; if OFF, world axes.")]
    [SerializeField] bool useLocalAxes = false;

    [Header("Axis 0 (REQUIRED)")]
    [Tooltip("Primary movement direction, e.g., (1,0,0) for +X")]
    [SerializeField] Vector3 axis0 = Vector3.right;
    [Tooltip("Distance to travel along Axis 0 (units)")]
    [SerializeField, Min(0f)] float distance0 = 5f;

    [Header("Axis 1 (OPTIONAL)")]
    [Tooltip("Enable a second axis (for X+Y corner paths)")]
    [SerializeField] bool useAxis1 = false;
    [Tooltip("Second movement direction, e.g., (0,1,0) for +Y")]
    [SerializeField] Vector3 axis1 = Vector3.up;
    [Tooltip("Distance to travel along Axis 1 (units)")]
    [SerializeField, Min(0f)] float distance1 = 3f;

    [Header("Corner Path Style (when using two axes)")]
    [Tooltip("If ON: move X then Y (no diagonal). If OFF: straight to corner (diagonal).")]
    [SerializeField] bool sequentialCorner = true;

    [Header("Ping-Pong Settings (PingPongToCorner mode)")]
    [Tooltip("Units/second while moving to targets")]
    [SerializeField, Min(0.01f)] float moveSpeed = 2f;
    [Tooltip("How many times to go End -> Origin (Origin->End->Origin = 1 cycle)")]
    [SerializeField, Min(1)] int cycles = 3;
    [SerializeField] bool snapToOriginAfter = true;

    [Header("Impulse Settings (ImpulseSingleAxis mode)")]
    [Tooltip("Strength of the shove along Axis 0")]
    [SerializeField, Min(0f)] float baseImpulse = 6f;
    [Tooltip("Clamp max linear speed after impulse (0 = no clamp)")]
    [SerializeField, Min(0f)] float maxSpeed = 0f;

    Rigidbody rb;
    Coroutine activeCo;
    Vector3 origin;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        origin = rb.position; //Starting spot becomes Origin
    }

    //Called by bullet
    public void Nudge(float amount)
    {
        switch (mode)
        {
            case MoveMode.PingPongToCorner:
                StartOrRestart(PingPongCycles(cycles));
                break;

            case MoveMode.ImpulseSingleAxis:
                ApplyImpulseOnAxis0(amount > 0f ? amount : baseImpulse);
                break;
        }
    }

    //Impulse (physics shove)
    void ApplyImpulseOnAxis0(float impulse)
    {
        Vector3 dir = GetAxisDir(axis0);
        if (dir.sqrMagnitude < 1e-6f || impulse <= 0f) return;
        rb.isKinematic = false; //ensure physics moves it
        rb.AddForce(dir * impulse, ForceMode.VelocityChange);
        if (maxSpeed > 0f && rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    //Ping-pong (scripted motion)
    IEnumerator PingPongCycles(int count)
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        Vector3 O = origin;
        Vector3 D0 = GetAxisDir(axis0) * Mathf.Max(0f, distance0);
        bool hasY = useAxis1 && axis1.sqrMagnitude > 1e-6f && distance1 > 0f;
        Vector3 D1 = hasY ? GetAxisDir(axis1) * Mathf.Max(0f, distance1) : Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            if (hasY && sequentialCorner)
            {
                //O -> X -> X+Y -> X -> O (axis-by-axis), then repeat
                yield return MoveTo(O + D0, moveSpeed);
                yield return MoveTo(O + D0 + D1, moveSpeed);
                yield return MoveTo(O + D0, moveSpeed);
                yield return MoveTo(O, moveSpeed);
            }
            else
            {
                //O -> Corner -> O (diagonal if two axes; straight if one)
                Vector3 end = O + D0 + D1;
                yield return MoveTo(end, moveSpeed);
                yield return MoveTo(O, moveSpeed);
            }
        }

        if (snapToOriginAfter) rb.MovePosition(O);
        activeCo = null;
    }

    IEnumerator MoveTo(Vector3 target, float speed)
    {
        const float eps = 0.001f;
        while ((rb.position - target).sqrMagnitude > eps * eps)
        {
            Vector3 next = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(next);
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(target);
    }

    //Helpers
    Vector3 GetAxisDir(Vector3 raw)
    {
        if (raw.sqrMagnitude < 1e-6f) return Vector3.zero;
        Vector3 dir = raw.normalized;
        return useLocalAxes ? transform.TransformDirection(dir) : dir;
    }

    void StartOrRestart(IEnumerator routine)
    {
        if (activeCo != null) StopCoroutine(activeCo);
        activeCo = StartCoroutine(routine);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var t = transform;
        Vector3 pos = Application.isPlaying ? origin : t.position;

        Vector3 d0 = axis0.sqrMagnitude < 1e-6f ? Vector3.zero : axis0.normalized;
        Vector3 d1 = axis1.sqrMagnitude < 1e-6f ? Vector3.zero : axis1.normalized;
        if (useLocalAxes)
        {
            d0 = t.TransformDirection(d0);
            d1 = t.TransformDirection(d1);
        }
        Vector3 X = pos + d0 * Mathf.Max(0f, distance0);
        Vector3 end = X + (useAxis1 ? d1 * Mathf.Max(0f, distance1) : Vector3.zero);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos, X);
        if (useAxis1)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(X, end);
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(end, 0.08f);
    }
#endif
}