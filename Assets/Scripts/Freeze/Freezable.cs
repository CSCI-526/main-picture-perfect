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

    private EffectHighlighter fx;

    public bool IsFrozen => isFrozen;

    void Awake()
    {
        fx = GetComponentInParent<EffectHighlighter>();
    }

    public void Freeze(float duration, string source = "default")
    {
        Debug.Log($"[Freezable] {name} frozen for {duration}s by {source}");
        if(!isFrozen)
        {
            isFrozen = true;
            fx?.Activate();
        }
        remaining = Mathf.Max(remaining, duration);
        OnFreeze(source);
    }

    protected virtual void OnFreeze(string source) { }

    public void Unfreeze()
    {
        if (!isFrozen) return;
        isFrozen = false;
        remaining = 0f;
        fx?.Deactivate();
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