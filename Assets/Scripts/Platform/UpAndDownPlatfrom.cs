using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Moves platform back and forth between two Y positions.
Inherits from Freezable.cs to allow freezing.
*/

public class UpAndDownPlatform : Freezable
{
    [Header("World Y positions to travel between")]
    public float yA = 5f;   //y - point1
    public float yB = 10f;  //y - point2

    [Header("Motion")]
    public float speed = 8f;
    public float waitAtEnds = 0.50f;   //Seconds to wait at each end

    Rigidbody rb;
    int dir = 1;              //+1 = up, -1 = down
    float waitTimer = 0f;     //Wait timer at each end
    
    // Public property to check if platform is moving upward
    public bool IsMovingUp => dir > 0 && waitTimer <= 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        var p = transform.position;
        float dA = Mathf.Abs(p.y - yA);
        float dB = Mathf.Abs(p.y - yB);

        float startY = (dA <= dB) ? yA : yB;
        Vector3 snapped = new Vector3(p.x, startY, p.z);

        rb.position = snapped;
        transform.position = snapped;

        dir = (Mathf.Abs(startY - yA) < 0.001f) ? +1 : -1;
    }

    protected override void Update()
    {
        //Keep freeze timer
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
        float targetY = (dir > 0) ? yB : yA;
        float step = speed * Time.fixedDeltaTime;

        float newY = Mathf.MoveTowards(pos.y, targetY, step);
        Vector3 next = new Vector3(pos.x, newY, pos.z);

        rb.MovePosition(next);

        if (Mathf.Abs(newY - targetY) <= 0.001f)
        {
            dir *= -1;
            waitTimer = waitAtEnds;
        }
    }

    protected override void OnFreeze()
    {
        base.OnFreeze();
        var lockComponent = GetComponent<FreezeTransformLock>();
        if (lockComponent)
        {
            lockComponent.SnapshotNow(); //Snapshot current transform state
        }
    }
}
