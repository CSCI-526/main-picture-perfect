using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class FreezablePendulum : MonoBehaviour, IFreezable
{
    [SerializeField] float defaultSeconds = 2.0f;

    Rigidbody rb;
    HingeJoint hinge;

    bool isFrozen;
    bool cachedUseMotor;
    Vector3 cachedAngularVelocity;
    Coroutine freezeCo;

    //Debugging purposes
    string lastSource = "default";

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();

        //Smooth render while swinging
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;
    }

    public bool IsFrozen => isFrozen;

    public void Freeze(float duration, string source = "default")
    {
        lastSource = source;

        //Restart/extend freeze if already frozen
        if (freezeCo != null) StopCoroutine(freezeCo);
        float t = duration > 0f ? duration : defaultSeconds;
        freezeCo = StartCoroutine(FreezeRoutine(t));
    }

    public void Unfreeze()
    {
        if (!isFrozen) return;
        if (freezeCo != null) StopCoroutine(freezeCo);
        freezeCo = null;
        InternalUnfreeze();
    }

    IEnumerator FreezeRoutine(float seconds)
    {
        InternalFreeze();
        yield return new WaitForSeconds(seconds);
        InternalUnfreeze();
        freezeCo = null;
    }

    void InternalFreeze()
    {
        if (isFrozen) return;
        isFrozen = true;

        cachedAngularVelocity = rb.angularVelocity;
        if (hinge != null)
        {
            cachedUseMotor = hinge.useMotor;
            hinge.useMotor = false;
        }

        //Solid stop/freeze
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;          //Prevent jitter/"chunkiness" while frozen
    }

    void InternalUnfreeze()
    {
        if (!isFrozen) return;

        //Resume physics
        rb.isKinematic = false;
        if (hinge != null) hinge.useMotor = cachedUseMotor;

        //Pick up where it left off
        rb.angularVelocity = cachedAngularVelocity;

        isFrozen = false;
    }
}