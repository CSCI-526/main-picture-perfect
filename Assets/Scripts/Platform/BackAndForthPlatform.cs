using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Moves platform closest to spawn back and forth along 2 x-axis points
Inherits from Freezable.cs to freeze movement when needed
*/

public class BackAndForthPlatform : Freezable
{
    [Header("World X positions to travel between")]
    public float xA = 34f;     //x-point 1
    public float xB = -34f;    //x-point 2

    [Header("Motion")]
    public float speed = 10f;          
    public float waitAtEnds = 0.25f;   //how long to pause for at each x-point

    Rigidbody rb;
    int dir = 1;               //+1 toward xB, -1 toward xA
    float waitTimer = 0f; ////Countdown while waiting at either end. When > 0, movement paused

    void Awake()
    {
        // Ensure we have a kinematic RB (recommended for moving platforms)
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        var p = transform.position; //Fetches current position

        //Finds which x-point is closest and starts from there
        float dA = Mathf.Abs(p.x - xA);
        float dB = Mathf.Abs(p.x - xB);

        Vector3 snapped = new Vector3(dA <= dB ? xA : xB, p.y, p.z);
        rb.position = snapped;  //Set RB position so physics sees it
        transform.position = snapped;

        dir = (Mathf.Abs(transform.position.x - xA) < 0.001f) ? +1 : -1;
    }

    protected override void Update()
    {
        //Keep freeze timer ticking;
        base.Update();
    }

    void FixedUpdate()
    {
        if (IsFrozen) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 pos = rb.position;
        float targetX = (dir > 0) ? xB : xA;
        float step = speed * Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(pos.x, targetX, step);
        Vector3 next = new Vector3(newX, pos.y, pos.z);
        rb.MovePosition(next);

        if (Mathf.Abs(newX - targetX) <= 0.001f)
        {
            dir *= -1;
            waitTimer = waitAtEnds;
        }
    }

    protected override void OnFreeze()
    {
        var lockComponent = GetComponent<FreezeTransformLock>();
        if (lockComponent)
        {
            lockComponent.SnapshotNow();
        }
    }
}