using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBall : Freezable
{
    private Collider col; //Collider of the ball
    private MeshRenderer rend; // Renderer of the ball

    // Manager reference to notify when frozen/unfrozen
    public TargetBallManager manager; //Reference to the manager that controls this ball

    protected void Awake()
    {
        col = GetComponent<Collider>(); 
        rend = GetComponent<MeshRenderer>();
    }

    protected override void OnFreeze()
    {
        //When frozen → disable rendering and collider
        if (rend) rend.enabled = false;
        if (col) col.enabled = false;
        // Notify manager that I am frozen
        if (manager != null)
        {
            manager.OnBallFrozen(this);
        }
    }

    protected override void OnUnfreeze()
    {
        //When unfrozen → enable rendering and collider
        if (rend) rend.enabled = true;
        if (col) col.enabled = true;

        // Notify manager that I am unfrozen
        if (manager != null)
        {
            manager.OnBallUnfrozen(this);
        }
    }
}
