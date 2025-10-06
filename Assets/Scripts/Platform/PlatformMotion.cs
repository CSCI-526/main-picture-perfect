using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMotion : MonoBehaviour
{
    public Vector3 WorldDeltaThisFrame { get; private set; }

    Vector3 _last; //Platform's position in previous frame

    void Awake()
    {
        _last = transform.position;
    }

    void LateUpdate()
    {
        //Calculates how far platform moved since last frame
        //And stores current position for use in the next frame
        WorldDeltaThisFrame = transform.position - _last;
        _last = transform.position;
    }
}

