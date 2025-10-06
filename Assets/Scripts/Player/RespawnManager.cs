using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] Transform currentSpawn; //Set by code developers
    CharacterController cc;

    void Awake() {
        cc = GetComponent<CharacterController>();
        //Teleport BEFORE PlayerController.Start() runs
        if (currentSpawn != null)
        {
            TeleportTo(currentSpawn, resetLook: true);
        }
        else
        {
            Debug.LogWarning("RespawnManager: No currentSpawn assigned.");
        } 
    }

    public void SetSpawn(Transform t)   //Sets new spawn point (needed if we want to implement checkpoints)
    {
        currentSpawn = t;
    }

    public void Respawn() {
        if (currentSpawn == null)
        {
            Debug.LogWarning("RespawnManager: No spawn set.");
            return;
        }
        TeleportTo(currentSpawn, resetLook:true);
    }

    void TeleportTo(Transform t, bool resetLook) {
        if (cc)
        {
            cc.enabled = false; //Disable CharacterController to move transform safely
        }

        transform.position = t.position; //Move player to spawn position

        //Set player's rotation to spawn's y rotation
        transform.rotation = Quaternion.Euler(0f, t.eulerAngles.y, 0f);

        if (cc)
        {
            cc.enabled = true; //Re-enable CharacterController
        }

        if (resetLook)
        {
            var pc = GetComponent<PlayerController>();
            if (pc)
            {
                pc.ResetLook(0f);
            }
        }
    }
}
