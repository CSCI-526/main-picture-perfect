using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 axis = Vector3.forward;  // Rotation axis
    public float speed = 45f;          

    void Update()
    {
        transform.Rotate(axis.normalized * speed * Time.deltaTime, Space.World);
    }
}
