using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerRideOnPlatforms : MonoBehaviour
{
    [Header("Detection")]
    public float probeDown = 0.45f;        //Dist to check below player for moving platforms
    public float minUpNormal = 0.55f;      //Enforces that surfaces have to be relatively flat
    public LayerMask groundMask = ~0;

    //True = Detection of valid platform on current frame
    public bool supportedThisFrame { get; private set; }

    CharacterController cc;
    public PlatformMotion current { get; private set; } //Current platform (if applicable)

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        //Detects what we're standing on this frame
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, probeDown, groundMask, QueryTriggerInteraction.Ignore)
            && hit.normal.y >= minUpNormal)
        {
            current = hit.collider.GetComponentInParent<PlatformMotion>();  //Tries to fetch PlatformMotion
            supportedThisFrame = true;
        }
        else    //No platform detected
        {
            current = null;
            supportedThisFrame = false;
        }

        //If on a moving platform, add its world-space delta
        if (current != null)
        {
            Vector3 delta = current.WorldDeltaThisFrame;
            if (delta.sqrMagnitude > 0f) cc.Move(delta);
        }
    }
}