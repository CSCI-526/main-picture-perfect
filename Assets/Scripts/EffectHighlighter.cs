using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EffectHighlighter : MonoBehaviour
{
    [Header("Assign the red material in Inspector")]
    [SerializeField] private Material effectMaterial;

    Renderer[] renderers;
    readonly List<Material[]> originalMats = new();
    readonly List<Material[]> effectMats = new();
    int activeCount = 0;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        originalMats.Clear(); effectMats.Clear();

        foreach (var r in renderers)
        {
            var originals = r.materials;        // per-instance
            originalMats.Add(originals);

            var slots = new Material[originals.Length];
            for (int i = 0; i < slots.Length; i++) slots[i] = effectMaterial;
            effectMats.Add(slots);
        }
    }

    void OnDisable()
    {
        if (renderers != null && originalMats.Count == renderers.Length)
            RestoreOriginals();
        activeCount = 0;
    }

    public void Activate()
    {
        if (effectMaterial == null || renderers == null) return;
        activeCount++;
        if (activeCount > 1) return;

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].materials = effectMats[i];
    }

    public void Deactivate()
    {
        if (renderers == null) return;
        activeCount = Mathf.Max(0, activeCount - 1);
        if (activeCount > 0) return;
        RestoreOriginals();
    }

    public void ActivateForSeconds(float seconds)
    {
        if (seconds <= 0f) { Activate(); Deactivate(); return; }
        Activate();
        StopAllCoroutines();
        StartCoroutine(DeactivateAfter(seconds));
    }

    System.Collections.IEnumerator DeactivateAfter(float t)
    {
        yield return new WaitForSeconds(t);
        Deactivate();
    }

    void RestoreOriginals()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].materials = originalMats[i];
    }
}