using UnityEngine;

public class GlobalFreezeTrigger : MonoBehaviour
{
    [Header("Freeze time in seconds")]
    public float freezeDuration = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TriggerFreeze();
        }
    }

    void TriggerFreeze()
    {
        var allFreezables = FindObjectsOfType<Freezable>();

        foreach (var f in allFreezables)
        {
            f.Freeze(freezeDuration, "global");//Clarify source of freeze
        }


        Debug.Log($"[GlobalFreezeTrigger] All Freezable has been frozen for {freezeDuration} seconds");
    }
}
