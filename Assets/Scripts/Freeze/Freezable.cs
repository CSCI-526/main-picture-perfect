using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Base class to freeze objects for a duration
*/

public class Freezable : MonoBehaviour, IFreezable
{
    [SerializeField] private bool isFrozen = false;
    [SerializeField] private float remaining = 0f;  //how many seconds remaining of being frozen

    public bool IsFrozen => isFrozen;

    public void Freeze(float duration)
    {
        Debug.Log($"[Freezable] {name} frozen for {duration}s");
        isFrozen = true;
        remaining = Mathf.Max(remaining, duration);
        OnFreeze();
    }

    public void Unfreeze()
    {
        isFrozen = false;
        remaining = 0f;
        OnUnfreeze();
    }

    //Optional override in child classes to change visuals, stop sounds, etc.
    protected virtual void OnFreeze() { }
    protected virtual void OnUnfreeze() { }

    //While frozen, count down remaining time each frame
    protected virtual void Update()
    {
        if (!isFrozen)
        {
            return;
        }

        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            Unfreeze();
        }
    }
}