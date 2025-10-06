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
    public float yA = 5f;   // y - point1
    public float yB = 10f;   // y - point2

    [Header("Motion")]
    public float speed = 8f;
    public float waitAtEnds = 0.50f;   // Seconds to wait at each end

    private int dir = 1;          // Direction: +1 = up, -1 = down
    private float waitTimer = 0f; // Wait timer at each end

    void Start()
    {
        var p = transform.position;
        float dA = Mathf.Abs(p.y - yA);
        float dB = Mathf.Abs(p.y - yB);

        // Initial position: snap to closest Y point
        transform.position = new Vector3(p.x, dA <= dB ? yA : yB, p.z);

        // Initial direction
        dir = (Mathf.Abs(transform.position.y - yA) < 0.001f) ? +1 : -1;
    }

    protected override void Update()
    {
        base.Update(); // Keep freeze timer ticking

        if (IsFrozen)
        {
            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        float targetY = (dir > 0) ? yB : yA;
        var pos = transform.position;

        // Move towards target Y position
        float newY = Mathf.MoveTowards(pos.y, targetY, speed * Time.deltaTime);
        transform.position = new Vector3(pos.x, newY, pos.z);

        // Check if reached target
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
            lockComponent.SnapshotNow(); // Snapshot current transform state
        }
    }
}
