using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Creates a "floor trigger" that respawns users if they fall off platforms back to spawn
Works in unison with the RespawnManager.cs 
*/

public class FloorRespawnTrigger : MonoBehaviour
{
    void Reset()   //Called when component (this script) is first added to a GO
    {
        var col = GetComponent<Collider>(); //Ensures GO has a collider
        col.isTrigger = true;   //Ensures collider is a trigger (to trigger respawn)

        if (!TryGetComponent<Rigidbody>(out var rb))    //Checks for RigidBody and configures its settings
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        else
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other) //Called when player collides with "death floor"
    {
        if (!other.CompareTag("Player"))
        {
            return; //if its not a player, we don't care
        }

        //Grab RespawnManager from the player and respawn
        var rm = other.GetComponent<RespawnManager>();
        if (rm != null)
        {
            rm.Respawn();
        }
    }
}
