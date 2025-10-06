using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Freezes an object's position and/or rotation
*/

public class FreezeTransformLock : MonoBehaviour
{
    public Freezable target; // leave empty to auto-find on this object/parents

    public bool lockPosition = true;
    public bool lockRotation = true;

    Vector3 frozenPos;
    Quaternion frozenRot;

    bool snapshotTaken = false;

    void Awake()
    {
        if (!target)
        {
            target = GetComponentInParent<Freezable>();
        }
    }

    void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        if (target.IsFrozen)
        {
            if (!snapshotTaken)
            {
                SnapshotNow(); // 用统一函数代替重复代码
            }

            if (lockPosition)
            {
                transform.position = frozenPos;
            }
            if (lockRotation)
            {
                transform.rotation = frozenRot;
            }
        }
        else
        {
            snapshotTaken = false;
        }
    }

    public void SnapshotNow()
    {
        frozenPos = transform.position;
        frozenRot = transform.rotation;
        snapshotTaken = true;
    }
}
