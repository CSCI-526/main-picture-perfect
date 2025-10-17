using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class HingeOscillator : MonoBehaviour
{
    public float speed = 120f;  //Deg/sec
    public float force = 500f;
    public float flipMargin = 2f; //Deg before the limit to flip

    HingeJoint hj;

    void Awake() {
        hj = GetComponent<HingeJoint>();
        var m = hj.motor; m.force = force; m.targetVelocity = speed; hj.motor = m;
        hj.useMotor = true; hj.useLimits = true;
    }

    void FixedUpdate() {
        float angle = hj.angle; //Relative angle in degrees
        var limits = hj.limits;
        var m = hj.motor;

        //Near high limit --> go negative; Near low limit --> go positive.
        if (angle > limits.max - flipMargin)       m.targetVelocity = -Mathf.Abs(speed);
        else if (angle < limits.min + flipMargin)  m.targetVelocity =  Mathf.Abs(speed);

        hj.motor = m;
    }
}