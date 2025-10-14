using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMotion : MonoBehaviour
{
    //Delta of current rendered frame (already interpolated by Rigidbody)
    public Vector3 WorldDeltaThisFrame { get; private set; }

    Rigidbody rb;
    Vector3 lastLatePos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //Smooths visuals
        if (rb) rb.interpolation = RigidbodyInterpolation.Interpolate;
        lastLatePos = transform.position;
        WorldDeltaThisFrame = Vector3.zero;
    }

    void LateUpdate()
    {
        Vector3 current = transform.position;
        WorldDeltaThisFrame = current - lastLatePos;
        lastLatePos = current;
    }
}

